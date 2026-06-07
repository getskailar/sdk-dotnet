using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>The full detail card for a model, including pricing, modalities, and aliases.</summary>
public sealed record Model : ModelSummary
{
    /// <summary>A longer description of the model.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>The supported input and output modalities.</summary>
    [JsonPropertyName("modalities")]
    public Modalities? Modalities { get; init; }

    /// <summary>The request parameters the model honors.</summary>
    [JsonPropertyName("supported_parameters")]
    public IReadOnlyList<string>? SupportedParameters { get; init; }

    /// <summary>The training knowledge cutoff, when published.</summary>
    [JsonPropertyName("knowledge_cutoff")]
    public string? KnowledgeCutoff { get; init; }

    /// <summary>The release date, when published.</summary>
    [JsonPropertyName("released_at")]
    public string? ReleasedAt { get; init; }

    /// <summary>A link to the model's documentation.</summary>
    [JsonPropertyName("documentation_url")]
    public string? DocumentationUrl { get; init; }

    /// <summary>Known aliases that also resolve to this model.</summary>
    [JsonPropertyName("aliases")]
    public IReadOnlyList<string>? Aliases { get; init; }
}

/// <summary>The input and output modalities a model supports.</summary>
public sealed record Modalities
{
    /// <summary>The accepted input modalities (for example <c>text</c>, <c>image</c>).</summary>
    [JsonPropertyName("input")]
    public IReadOnlyList<string>? Input { get; init; }

    /// <summary>The produced output modalities.</summary>
    [JsonPropertyName("output")]
    public IReadOnlyList<string>? Output { get; init; }
}
