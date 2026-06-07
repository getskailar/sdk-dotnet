using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A request to synthesize speech from text.</summary>
public sealed record SpeechRequest
{
    /// <summary>The text to speak. Up to 4000 characters.</summary>
    [JsonPropertyName("input")]
    public required string Input { get; init; }

    /// <summary>The voice to use. Defaults to <see cref="SkailarVoice.Nova"/>.</summary>
    [JsonPropertyName("voice")]
    public SkailarVoice? Voice { get; init; }
}
