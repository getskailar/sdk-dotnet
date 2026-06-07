using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>
/// The content of a chat message: either plain text or a list of structured
/// <see cref="ContentPart"/> values. Serializes to a bare string or a JSON array
/// to match the OpenAI-compatible wire format.
/// </summary>
[JsonConverter(typeof(MessageContentConverter))]
public sealed class MessageContent
{
    /// <summary>The plain-text content, when this value is a simple string.</summary>
    public string? Text { get; init; }

    /// <summary>The structured content parts, when this value is a list.</summary>
    public IReadOnlyList<ContentPart>? Parts { get; init; }

    /// <summary>Creates message content from a plain string.</summary>
    /// <param name="text">The text content.</param>
    public static MessageContent FromText(string text) => new() { Text = text };

    /// <summary>Creates message content from structured parts.</summary>
    /// <param name="parts">The content parts.</param>
    public static MessageContent FromParts(IReadOnlyList<ContentPart> parts) => new() { Parts = parts };

    /// <summary>Returns the text, concatenating the text of any structured parts.</summary>
    public string GetText() =>
        Text ?? string.Concat(Parts?.OfType<TextPart>().Select(static p => p.Value) ?? []);

    /// <summary>Implicitly wraps a string as text content.</summary>
    /// <param name="text">The text content.</param>
    public static implicit operator MessageContent(string text) => FromText(text);
}
