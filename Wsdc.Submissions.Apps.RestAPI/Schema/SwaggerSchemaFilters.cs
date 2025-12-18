using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wsdc.Submissions.Apps.RestAPI.Schema;

/// <summary>
/// Swagger schema filter that provides proper documentation for the Judge.Score property
/// For non-finals: "10", "4.5", "4.3", "4.2", "0"
/// For finals: positive integer as string (e.g., "1", "2", "3")
/// </summary>
public class JudgeScoreSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(Wsdc.Submissions.Models.Dtos.Judge))
        {
            return;
        }

        // Find the score property and update its schema
        if (schema.Properties.TryGetValue("score", out var scoreSchema))
        {
            scoreSchema.Type = "string";
            scoreSchema.Nullable = false;
            scoreSchema.Description = "Score value: For non-finals use '10', '4.5', '4.3', '4.2', or '0'. For finals use positive integer as string (e.g., '1', '2', '3')";
            scoreSchema.OneOf = new List<OpenApiSchema>
            {
                new OpenApiSchema
                {
                    Type = "string",
                    Description = "Score for non-final rounds (Prelims, Quarters, Semis)",
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("10"),
                        new OpenApiString("4.5"),
                        new OpenApiString("4.3"),
                        new OpenApiString("4.2"),
                        new OpenApiString("0")
                    }
                },
                new OpenApiSchema
                {
                    Type = "string",
                    Description = "Placement score for final rounds (positive integer as string, 1 or greater)",
                    Pattern = "^[1-9][0-9]*$"
                }
            };
        }
    }
}

