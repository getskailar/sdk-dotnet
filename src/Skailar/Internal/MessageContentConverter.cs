using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar.Internal;

/// <summary>Serializes <see cref="MessageContent"/> as either a bare string or an array of parts.</summary>
internal sealed class MessageContentConverter : JsonConverter<MessageContent>
{
    public override MessageContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                return MessageContent.FromText(reader.GetString()!);
            case JsonTokenType.StartArray:
                var parts = JsonSerializer.Deserialize(ref reader, SkailarJsonContext.Default.IReadOnlyListContentPart);
                return MessageContent.FromParts(parts ?? []);
            default:
                throw new JsonException($"Unexpected token '{reader.TokenType}' for message content.");
        }
    }

    public override void Write(Utf8JsonWriter writer, MessageContent value, JsonSerializerOptions options)
    {
        if (value.Parts is { } parts)
        {
            JsonSerializer.Serialize(writer, parts, SkailarJsonContext.Default.IReadOnlyListContentPart);
            return;
        }

        writer.WriteStringValue(value.Text ?? string.Empty);
    }
}
