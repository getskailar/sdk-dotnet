using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A request to create a chat completion.</summary>
public sealed record ChatCompletionRequest
{
    /// <summary>The model identifier or alias. See <c>SkailarModels</c> or <c>GET /v1/models</c>.</summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>The conversation so far.</summary>
    [JsonPropertyName("messages")]
    public required IReadOnlyList<ChatMessage> Messages { get; init; }

    /// <summary>When <see langword="true"/>, the response is streamed as SSE chunks.</summary>
    [JsonPropertyName("stream")]
    public bool? Stream { get; init; }

    /// <summary>The maximum number of tokens to generate.</summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; }

    /// <summary>The sampling temperature, between 0 and 2.</summary>
    [JsonPropertyName("temperature")]
    public float? Temperature { get; init; }

    /// <summary>The nucleus sampling probability mass, between 0 and 1.</summary>
    [JsonPropertyName("top_p")]
    public float? TopP { get; init; }

    /// <summary>The reasoning budget for reasoning-capable models.</summary>
    [JsonPropertyName("reasoning_effort")]
    public ReasoningEffort? ReasoningEffort { get; init; }

    /// <summary>The function-calling tools the model may use.</summary>
    [JsonPropertyName("tools")]
    public IReadOnlyList<Tool>? Tools { get; init; }

    /// <summary>Controls whether and how the model calls tools.</summary>
    [JsonPropertyName("tool_choice")]
    public ToolChoice? ToolChoice { get; init; }

    /// <summary>Constrains the response format.</summary>
    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; init; }

    /// <summary>The number of completions to generate.</summary>
    [JsonPropertyName("n")]
    public int? N { get; init; }

    /// <summary>Penalizes new tokens based on whether they already appear.</summary>
    [JsonPropertyName("presence_penalty")]
    public float? PresencePenalty { get; init; }

    /// <summary>Penalizes new tokens based on their existing frequency.</summary>
    [JsonPropertyName("frequency_penalty")]
    public float? FrequencyPenalty { get; init; }

    /// <summary>A map of token bias adjustments.</summary>
    [JsonPropertyName("logit_bias")]
    public JsonElement? LogitBias { get; init; }

    /// <summary>A stable end-user identifier for abuse monitoring.</summary>
    [JsonPropertyName("user")]
    public string? User { get; init; }

    /// <summary>A seed for best-effort deterministic sampling.</summary>
    [JsonPropertyName("seed")]
    public int? Seed { get; init; }

    /// <summary>One or more sequences at which generation stops.</summary>
    [JsonPropertyName("stop")]
    public IReadOnlyList<string>? Stop { get; init; }
}
