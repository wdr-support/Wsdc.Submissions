using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Generation;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Apps.RestAPI.Controllers;

/// <summary>
/// Controller for serving JSON schemas publicly
/// </summary>
[ApiController]
[Route("api/schemas")]
public class SchemasController : ControllerBase
{
    private readonly ILogger<SchemasController> _logger;
    private static readonly object _cacheLock = new object();
    private static JsonSchema? _cachedSchema = null;

    public SchemasController(ILogger<SchemasController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the event data JSON schema
    /// </summary>
    /// <returns>JSON schema for event data validation</returns>
    /// <response code="200">Schema retrieved successfully</response>
    [HttpGet("results")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetEventDataSchema()
    {
        _logger.LogInformation("Received request for event data schema");

        try
        {
            // Use cached schema if available
            if (_cachedSchema == null)
            {
                lock (_cacheLock)
                {
                    if (_cachedSchema == null)
                    {
                        _logger.LogInformation("Generating JSON schema from EventSubmissionRequest model");

                        var settings = new SystemTextJsonSchemaGeneratorSettings
                        {
                            SchemaType = SchemaType.JsonSchema,
                            GenerateAbstractProperties = false,
                            FlattenInheritanceHierarchy = true,
                            DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull
                        };

                        _cachedSchema = JsonSchema.FromType<EventResultsRequest>(settings);

                        // Add metadata
                        _cachedSchema.Title = "WSDC Event Data Submission Schema";
                        _cachedSchema.Description = "JSON Schema for validating WSDC event data submissions";
                    }
                }
            }

            var schemaJson = _cachedSchema.ToJson();

            // Post-process the JSON to convert enums from integer to string format
            schemaJson = PostProcessEnumJson(schemaJson);

            _logger.LogInformation("Event data schema retrieved successfully");

            return Content(schemaJson, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating event data schema");
            return StatusCode(500, new { error = "Failed to generate schema" });
        }
    }

    /// <summary>
    /// Post-process the JSON schema to convert enums from integer to string format
    /// and convert definition names to camelCase
    /// </summary>
    private static string PostProcessEnumJson(string schemaJson)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(schemaJson);
        using var stream = new System.IO.MemoryStream();
        using var writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { Indented = true });

        ProcessJsonElement(doc.RootElement, writer, isDefinitionsObject: false);

        writer.Flush();
        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Converts a PascalCase string to camelCase
    /// </summary>
    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    private static void ProcessJsonElement(System.Text.Json.JsonElement element, System.Text.Json.Utf8JsonWriter writer, bool isDefinitionsObject)
    {
        switch (element.ValueKind)
        {
            case System.Text.Json.JsonValueKind.Object:
                ProcessJsonObject(element, writer, isDefinitionsObject);
                break;
            case System.Text.Json.JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    ProcessJsonElement(item, writer, isDefinitionsObject: false);
                }
                writer.WriteEndArray();
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static void ProcessJsonObject(System.Text.Json.JsonElement obj, System.Text.Json.Utf8JsonWriter writer, bool isDefinitionsObject)
    {
        writer.WriteStartObject();

        // Check if this is an enum definition (has x-enumNames)
        bool isEnumDef = false;
        List<string>? enumNames = null;

        foreach (var prop in obj.EnumerateObject())
        {
            if (prop.Name == "x-enumNames" && prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                isEnumDef = true;
                enumNames = prop.Value.EnumerateArray()
                    .Select(e => e.GetString())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()!;
            }
        }

        foreach (var prop in obj.EnumerateObject())
        {
            if (isEnumDef && enumNames != null)
            {
                // Skip x-enumNames (we've extracted the values)
                if (prop.Name == "x-enumNames")
                    continue;

                // Replace type: integer with type: string
                if (prop.Name == "type" && prop.Value.GetString() == "integer")
                {
                    writer.WriteString("type", "string");
                    continue;
                }

                // Replace enum array with string values
                if (prop.Name == "enum" && prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    writer.WritePropertyName("enum");
                    writer.WriteStartArray();
                    foreach (var name in enumNames)
                    {
                        writer.WriteStringValue(name);
                    }
                    writer.WriteEndArray();
                    continue;
                }
            }

            // Handle definitions object - convert keys to camelCase
            if (prop.Name == "definitions" && prop.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                writer.WritePropertyName("definitions");
                ProcessJsonElement(prop.Value, writer, isDefinitionsObject: true);
                continue;
            }

            // Handle $ref values - convert definition references to camelCase
            if (prop.Name == "$ref" && prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var refValue = prop.Value.GetString() ?? string.Empty;
                if (refValue.StartsWith("#/definitions/"))
                {
                    var defName = refValue.Substring("#/definitions/".Length);
                    var camelCaseRef = "#/definitions/" + ToCamelCase(defName);
                    writer.WriteString("$ref", camelCaseRef);
                    continue;
                }
            }

            // If inside definitions object, convert property name to camelCase
            if (isDefinitionsObject)
            {
                writer.WritePropertyName(ToCamelCase(prop.Name));
                ProcessJsonElement(prop.Value, writer, isDefinitionsObject: false);
            }
            else
            {
                // Write other properties normally
                writer.WritePropertyName(prop.Name);
                ProcessJsonElement(prop.Value, writer, isDefinitionsObject: false);
            }
        }

        writer.WriteEndObject();
    }
}

