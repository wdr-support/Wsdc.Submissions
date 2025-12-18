using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wsdc.Submissions.Core.Converters;

/// <summary>
/// JSON converter factory that creates lenient enum converters for both nullable and non-nullable enums.
/// Invalid enum values deserialize to default (non-nullable) or null (nullable) instead of throwing exceptions.
/// </summary>
public class LenientEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Handle both Enum and Nullable<Enum>
        if (typeToConvert.IsEnum)
        {
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        return underlyingType != null && underlyingType.IsEnum;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);

        if (underlyingType != null)
        {
            // Nullable enum
            var converterType = typeof(LenientNullableEnumConverter<>).MakeGenericType(underlyingType);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
        else
        {
            // Non-nullable enum
            var converterType = typeof(LenientEnumConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}

/// <summary>
/// Lenient enum converter for non-nullable enums.
/// Invalid values deserialize to default(TEnum) - FluentValidation should check for valid values.
/// </summary>
public class LenientEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return default;
            }

            if (Enum.TryParse<TEnum>(stringValue, ignoreCase: false, out var result) && Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }

            return default;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(TEnum), intValue))
            {
                return (TEnum)(object)intValue;
            }
            return default;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Lenient enum converter for nullable enums.
/// Invalid values deserialize to null - FluentValidation should validate when value is required.
/// </summary>
public class LenientNullableEnumConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
{
    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            if (Enum.TryParse<TEnum>(stringValue, ignoreCase: false, out var result) && Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }

            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(TEnum), intValue))
            {
                return (TEnum)(object)intValue;
            }
            return null;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

