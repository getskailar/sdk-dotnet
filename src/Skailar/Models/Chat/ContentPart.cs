using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A single part of a structured (multimodal) message content.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextPart), "text")]
[JsonDerivedType(typeof(ImagePart), "image_url")]
public abstract record ContentPart
{
    /// <summary>Creates a text content part.</summary>
    /// <param name="text">The text fragment.</param>
    public static TextPart Text(string text) => new() { Value = text };

    /// <summary>Creates an image content part from a URL or <c>data:</c> URI.</summary>
    /// <param name="url">An HTTPS URL or a <c>data:</c> URI.</param>
    /// <param name="detail">Optional fidelity hint (<c>low</c>, <c>high</c>, or <c>auto</c>).</param>
    public static ImagePart Image(string url, string? detail = null) =>
        new() { ImageUrl = new ImageUrl { Url = url, Detail = detail } };
}

/// <summary>A text fragment within a structured message.</summary>
public sealed record TextPart : ContentPart
{
    /// <summary>The text fragment.</summary>
    [JsonPropertyName("text")]
    public required string Value { get; init; }
}

/// <summary>An image reference within a structured message.</summary>
public sealed record ImagePart : ContentPart
{
    /// <summary>The image location and fidelity hint.</summary>
    [JsonPropertyName("image_url")]
    public required ImageUrl ImageUrl { get; init; }
}

/// <summary>A reference to an image by URL, with an optional fidelity hint.</summary>
public sealed record ImageUrl
{
    /// <summary>An HTTPS URL or a <c>data:</c> URI pointing at the image.</summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>The requested fidelity: <c>low</c>, <c>high</c>, or <c>auto</c>.</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; init; }
}
