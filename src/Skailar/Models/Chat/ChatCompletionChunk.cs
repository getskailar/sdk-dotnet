using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>An incremental chunk of a streamed chat completion.</summary>
public sealed record ChatCompletionChunk
{
    /// <summary>The completion identifier, stable across the stream.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The object type. Always <c>chat.completion.chunk</c>.</summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "chat.completion.chunk";

    /// <summary>The creation time, as Unix epoch seconds.</summary>
    [JsonPropertyName("created")]
    public long Created { get; init; }

    /// <summary>The model that produced the chunk.</summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>The per-choice deltas carried by this chunk.</summary>
    [JsonPropertyName("choices")]
    public required IReadOnlyList<ChunkChoice> Choices { get; init; }

    /// <summary>Token accounting, present only on the final chunk for some providers.</summary>
    [JsonPropertyName("usage")]
    public Usage? Usage { get; init; }
}

/// <summary>A single choice's delta within a streamed chunk.</summary>
public sealed record ChunkChoice
{
    /// <summary>The index of this choice in the list.</summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>The incremental content for this choice.</summary>
    [JsonPropertyName("delta")]
    public required Delta Delta { get; init; }

    /// <summary>Why generation stopped, present on the terminal chunk for this choice.</summary>
    [JsonPropertyName("finish_reason")]
    public FinishReason? FinishReason { get; init; }
}

/// <summary>The incremental fields delivered in a streamed chunk.</summary>
public sealed record Delta
{
    /// <summary>The author role, present on the first delta.</summary>
    [JsonPropertyName("role")]
    public ChatRole? Role { get; init; }

    /// <summary>The incremental text fragment.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>The incremental reasoning trace, for reasoning-capable models.</summary>
    [JsonPropertyName("reasoning_content")]
    public string? ReasoningContent { get; init; }

    /// <summary>Incremental tool-call fragments.</summary>
    [JsonPropertyName("tool_calls")]
    public IReadOnlyList<ToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>An incremental tool-call fragment within a streamed delta.</summary>
public sealed record ToolCallDelta
{
    /// <summary>The index of the tool call being assembled.</summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }

    /// <summary>The tool-call identifier, present on the first fragment.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The tool type, present on the first fragment.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>The incremental function name and arguments.</summary>
    [JsonPropertyName("function")]
    public FunctionCallDelta? Function { get; init; }
}

/// <summary>An incremental function name and arguments fragment.</summary>
public sealed record FunctionCallDelta
{
    /// <summary>The function name, present on the first fragment.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>An incremental slice of the JSON-encoded arguments string.</summary>
    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
