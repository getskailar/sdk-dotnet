using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>
/// Controls whether and how the model calls tools: a mode string
/// (<c>auto</c>, <c>none</c>, <c>required</c>) or a specific named function.
/// </summary>
[JsonConverter(typeof(ToolChoiceConverter))]
public sealed class ToolChoice
{
    /// <summary>The mode string, when the choice is a simple mode.</summary>
    public string? Mode { get; init; }

    /// <summary>The forced function name, when the choice targets a specific function.</summary>
    public string? FunctionName { get; init; }

    /// <summary>Let the model decide whether to call a tool.</summary>
    public static ToolChoice Auto { get; } = new() { Mode = "auto" };

    /// <summary>Forbid tool calls.</summary>
    public static ToolChoice None { get; } = new() { Mode = "none" };

    /// <summary>Require the model to call at least one tool.</summary>
    public static ToolChoice Required { get; } = new() { Mode = "required" };

    /// <summary>Force the model to call a specific function.</summary>
    /// <param name="name">The function name to call.</param>
    public static ToolChoice Function(string name) => new() { FunctionName = name };
}
