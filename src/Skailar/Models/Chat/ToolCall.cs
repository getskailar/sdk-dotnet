using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A tool invocation requested by the assistant.</summary>
public sealed record ToolCall
{
    /// <summary>The unique identifier of this tool call.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The tool type. Always <c>function</c>.</summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    /// <summary>The function the assistant wants to call.</summary>
    [JsonPropertyName("function")]
    public required FunctionCall Function { get; init; }
}

/// <summary>The function name and JSON-encoded arguments of a <see cref="ToolCall"/>.</summary>
public sealed record FunctionCall
{
    /// <summary>The function name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>The function arguments, encoded as a JSON string.</summary>
    [JsonPropertyName("arguments")]
    public required string Arguments { get; init; }
}
