using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A completed (non-streamed) chat completion.</summary>
public sealed record ChatCompletionResponse
{
    /// <summary>The unique completion identifier.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The object type. Always <c>chat.completion</c>.</summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "chat.completion";

    /// <summary>The creation time, as Unix epoch seconds.</summary>
    [JsonPropertyName("created")]
    public long Created { get; init; }

    /// <summary>The model that produced the completion.</summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>The generated choices.</summary>
    [JsonPropertyName("choices")]
    public required IReadOnlyList<Choice> Choices { get; init; }

    /// <summary>Token accounting for the request.</summary>
    [JsonPropertyName("usage")]
    public Usage? Usage { get; init; }
}

/// <summary>A single generated choice within a completion.</summary>
public sealed record Choice
{
    /// <summary>The index of this choice in the list.</summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>The generated message.</summary>
    [JsonPropertyName("message")]
    public required ResponseMessage Message { get; init; }

    /// <summary>Why generation stopped for this choice.</summary>
    [JsonPropertyName("finish_reason")]
    public FinishReason? FinishReason { get; init; }
}

/// <summary>An assistant message returned in a completion choice.</summary>
public sealed record ResponseMessage
{
    /// <summary>The author role. Always <see cref="ChatRole.Assistant"/>.</summary>
    [JsonPropertyName("role")]
    public ChatRole Role { get; init; } = ChatRole.Assistant;

    /// <summary>The message text.</summary>
    [JsonPropertyName("content")]
    public MessageContent? Content { get; init; }

    /// <summary>The reasoning trace, for reasoning-capable models.</summary>
    [JsonPropertyName("reasoning_content")]
    public string? ReasoningContent { get; init; }

    /// <summary>Tool calls requested by the assistant.</summary>
    [JsonPropertyName("tool_calls")]
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }
}
