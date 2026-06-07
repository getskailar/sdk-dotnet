using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A catalog entry describing an available model.</summary>
public record ModelSummary
{
    /// <summary>The canonical model identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The object type. Always <c>model</c>.</summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "model";

    /// <summary>The creation time, as Unix epoch seconds.</summary>
    [JsonPropertyName("created")]
    public long Created { get; init; }

    /// <summary>The provider that owns the model.</summary>
    [JsonPropertyName("owned_by")]
    public required string OwnedBy { get; init; }

    /// <summary>The human-friendly display name.</summary>
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; init; }

    /// <summary>The maximum context window in tokens.</summary>
    [JsonPropertyName("context_window")]
    public int ContextWindow { get; init; }

    /// <summary>The maximum number of output tokens.</summary>
    [JsonPropertyName("max_output_tokens")]
    public int MaxOutputTokens { get; init; }

    /// <summary>The model's feature flags.</summary>
    [JsonPropertyName("capabilities")]
    public required ModelCapabilities Capabilities { get; init; }

    /// <summary>The model's per-token pricing.</summary>
    [JsonPropertyName("pricing")]
    public required ModelPricing Pricing { get; init; }

    /// <summary>The lifecycle status: <c>active</c>, <c>preview</c>, or <c>deprecated</c>.</summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }
}

/// <summary>The feature flags advertised by a model.</summary>
public sealed record ModelCapabilities
{
    /// <summary>Whether the model supports streamed responses.</summary>
    [JsonPropertyName("streaming")]
    public bool Streaming { get; init; }

    /// <summary>Whether the model supports function/tool calling.</summary>
    [JsonPropertyName("tool_calls")]
    public bool ToolCalls { get; init; }

    /// <summary>Whether the model accepts image inputs.</summary>
    [JsonPropertyName("vision")]
    public bool Vision { get; init; }

    /// <summary>Whether the model supports JSON-mode output.</summary>
    [JsonPropertyName("json_mode")]
    public bool JsonMode { get; init; }

    /// <summary>Whether the model exposes a reasoning budget, when known.</summary>
    [JsonPropertyName("reasoning")]
    public bool? Reasoning { get; init; }
}

/// <summary>The per-token pricing of a model.</summary>
public sealed record ModelPricing
{
    /// <summary>The input price per million tokens.</summary>
    [JsonPropertyName("input_per_mtok")]
    public double InputPerMTok { get; init; }

    /// <summary>The output price per million tokens.</summary>
    [JsonPropertyName("output_per_mtok")]
    public double OutputPerMTok { get; init; }

    /// <summary>The ISO currency code for the prices.</summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }
}
