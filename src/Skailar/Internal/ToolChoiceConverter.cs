using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar.Internal;

/// <summary>Serializes <see cref="ToolChoice"/> as either a mode string or a function-selector object.</summary>
internal sealed class ToolChoiceConverter : JsonConverter<ToolChoice>
{
    public override ToolChoice? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                return new ToolChoice { Mode = reader.GetString() };
            case JsonTokenType.StartObject:
                return ReadObject(ref reader);
            default:
                throw new JsonException($"Unexpected token '{reader.TokenType}' for tool_choice.");
        }
    }

    private static ToolChoice ReadObject(ref Utf8JsonReader reader)
    {
        string? functionName = null;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            bool isFunction = reader.ValueTextEquals("function");
            reader.Read();
            if (isFunction && reader.TokenType == JsonTokenType.StartObject)
            {
                functionName = ReadFunctionName(ref reader);
            }
            else
            {
                reader.Skip();
            }
        }

        return new ToolChoice { FunctionName = functionName };
    }

    private static string? ReadFunctionName(ref Utf8JsonReader reader)
    {
        string? name = null;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            bool isName = reader.ValueTextEquals("name");
            reader.Read();
            if (isName)
            {
                name = reader.GetString();
            }
            else
            {
                reader.Skip();
            }
        }

        return name;
    }

    public override void Write(Utf8JsonWriter writer, ToolChoice value, JsonSerializerOptions options)
    {
        if (value.FunctionName is { } name)
        {
            writer.WriteStartObject();
            writer.WriteString("type", "function");
            writer.WriteStartObject("function");
            writer.WriteString("name", name);
            writer.WriteEndObject();
            writer.WriteEndObject();
            return;
        }

        writer.WriteStringValue(value.Mode ?? "auto");
    }
}
