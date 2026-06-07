using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A request to generate one or more images from a prompt.</summary>
public sealed record ImageGenerationRequest
{
    /// <summary>The image model identifier.</summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>The text prompt describing the desired image.</summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>The number of images to generate (1–10).</summary>
    [JsonPropertyName("n")]
    public int? N { get; init; }

    /// <summary>The image dimensions, for example <c>1024x1024</c>.</summary>
    [JsonPropertyName("size")]
    public string? Size { get; init; }

    /// <summary>The provider-specific quality setting, for example <c>hd</c>.</summary>
    [JsonPropertyName("quality")]
    public string? Quality { get; init; }

    /// <summary>The provider-specific background setting, for example <c>transparent</c>.</summary>
    [JsonPropertyName("background")]
    public string? Background { get; init; }
}

/// <summary>The result of an image generation request.</summary>
public sealed record ImageGenerationResponse
{
    /// <summary>The creation time, as Unix epoch seconds.</summary>
    [JsonPropertyName("created")]
    public long Created { get; init; }

    /// <summary>The generated images.</summary>
    [JsonPropertyName("data")]
    public required IReadOnlyList<GeneratedImage> Data { get; init; }
}

/// <summary>A single generated image.</summary>
public sealed record GeneratedImage
{
    /// <summary>The URL of the generated image, when returned as a link.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>The base64-encoded image bytes, when returned inline.</summary>
    [JsonPropertyName("b64_json")]
    public string? B64Json { get; init; }

    /// <summary>The prompt as revised by the provider, when applicable.</summary>
    [JsonPropertyName("revised_prompt")]
    public string? RevisedPrompt { get; init; }
}
