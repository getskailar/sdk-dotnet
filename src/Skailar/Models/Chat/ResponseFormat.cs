using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>Constrains the shape of the model's response, in the OpenAI-compatible form.</summary>
public sealed record ResponseFormat
{
    /// <summary>The format type, such as <c>text</c>, <c>json_object</c>, or <c>json_schema</c>.</summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>The JSON Schema definition, when <see cref="Type"/> is <c>json_schema</c>.</summary>
    [JsonPropertyName("json_schema")]
    public JsonElement? JsonSchema { get; init; }

    /// <summary>Requests a free-form text response.</summary>
    public static ResponseFormat Text { get; } = new() { Type = "text" };

    /// <summary>Requests any valid JSON object.</summary>
    public static ResponseFormat JsonObject { get; } = new() { Type = "json_object" };
}
