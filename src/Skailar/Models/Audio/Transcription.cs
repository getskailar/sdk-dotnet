using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A request to transcribe a base64-encoded audio clip.</summary>
public sealed record TranscriptionRequest
{
    /// <summary>The base64-encoded audio bytes, without a <c>data:</c> prefix.</summary>
    [JsonPropertyName("base64")]
    public required string Base64 { get; init; }

    /// <summary>The MIME type of the audio. Defaults to <see cref="SkailarMime.Wav"/>.</summary>
    [JsonPropertyName("mime")]
    public SkailarMime? Mime { get; init; }
}

/// <summary>The result of an audio transcription.</summary>
public sealed record TranscriptionResponse
{
    /// <summary>The transcribed text.</summary>
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}
