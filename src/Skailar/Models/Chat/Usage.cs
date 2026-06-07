using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>Token accounting for a chat completion.</summary>
public sealed record Usage
{
    /// <summary>Tokens consumed by the prompt.</summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    /// <summary>Tokens produced in the completion.</summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    /// <summary>The sum of prompt and completion tokens.</summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
