using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A function-calling tool definition, in the OpenAI-compatible shape.</summary>
public sealed record Tool
{
    /// <summary>The tool type. Always <c>function</c>.</summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    /// <summary>The function this tool exposes.</summary>
    [JsonPropertyName("function")]
    public required FunctionDefinition Function { get; init; }

    /// <summary>Creates a function tool.</summary>
    /// <param name="name">The function name.</param>
    /// <param name="description">An optional human-readable description.</param>
    /// <param name="parameters">An optional JSON Schema describing the arguments.</param>
    public static Tool CreateFunction(string name, string? description = null, JsonElement? parameters = null) =>
        new() { Function = new FunctionDefinition { Name = name, Description = description, Parameters = parameters } };
}

/// <summary>The function exposed by a <see cref="Tool"/>.</summary>
public sealed record FunctionDefinition
{
    /// <summary>The function name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>A human-readable description of what the function does.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>A JSON Schema object describing the function arguments.</summary>
    [JsonPropertyName("parameters")]
    public JsonElement? Parameters { get; init; }
}
