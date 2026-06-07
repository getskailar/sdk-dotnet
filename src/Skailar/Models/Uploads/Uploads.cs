using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A request to upload a base64-encoded image to Skailar storage.</summary>
public sealed record UploadImageRequest
{
    /// <summary>The base64-encoded image bytes, without a <c>data:</c> prefix.</summary>
    [JsonPropertyName("base64")]
    public required string Base64 { get; init; }

    /// <summary>The image content type.</summary>
    [JsonPropertyName("content_type")]
    public required ImageContentType ContentType { get; init; }
}

/// <summary>A request to upload a base64-encoded document to Skailar storage.</summary>
public sealed record UploadFileRequest
{
    /// <summary>The base64-encoded document bytes, without a <c>data:</c> prefix.</summary>
    [JsonPropertyName("base64")]
    public required string Base64 { get; init; }

    /// <summary>The document content type.</summary>
    [JsonPropertyName("content_type")]
    public required FileContentType ContentType { get; init; }
}

/// <summary>The stored-asset URL returned by an upload.</summary>
public sealed record UploadResponse
{
    /// <summary>The Skailar-relative URL of the stored asset.</summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>The content type of the stored asset.</summary>
    [JsonPropertyName("content_type")]
    public required string ContentType { get; init; }
}
