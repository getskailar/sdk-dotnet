using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A single message in a chat conversation.</summary>
public sealed record ChatMessage
{
    /// <summary>The author role.</summary>
    [JsonPropertyName("role")]
    public required ChatRole Role { get; init; }

    /// <summary>The message content, as text or structured parts.</summary>
    [JsonPropertyName("content")]
    public MessageContent? Content { get; init; }

    /// <summary>Tool calls requested by the assistant, when present.</summary>
    [JsonPropertyName("tool_calls")]
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }

    /// <summary>The identifier of the tool call this message responds to. Required when <see cref="Role"/> is <see cref="ChatRole.Tool"/>.</summary>
    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; init; }

    /// <summary>Creates a system message.</summary>
    /// <param name="content">The instruction text.</param>
    public static ChatMessage System(string content) =>
        new() { Role = ChatRole.System, Content = MessageContent.FromText(content) };

    /// <summary>Creates a user message from plain text.</summary>
    /// <param name="content">The user text.</param>
    public static ChatMessage User(string content) =>
        new() { Role = ChatRole.User, Content = MessageContent.FromText(content) };

    /// <summary>Creates a user message from structured (multimodal) parts.</summary>
    /// <param name="parts">The content parts.</param>
    public static ChatMessage User(IReadOnlyList<ContentPart> parts) =>
        new() { Role = ChatRole.User, Content = MessageContent.FromParts(parts) };

    /// <summary>Creates an assistant message.</summary>
    /// <param name="content">The assistant text.</param>
    public static ChatMessage Assistant(string content) =>
        new() { Role = ChatRole.Assistant, Content = MessageContent.FromText(content) };

    /// <summary>Creates a tool result message.</summary>
    /// <param name="toolCallId">The identifier of the originating tool call.</param>
    /// <param name="content">The tool result, as text.</param>
    public static ChatMessage Tool(string toolCallId, string content) =>
        new() { Role = ChatRole.Tool, ToolCallId = toolCallId, Content = MessageContent.FromText(content) };
}
