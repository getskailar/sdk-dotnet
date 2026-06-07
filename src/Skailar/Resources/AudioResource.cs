using Skailar.Internal;

namespace Skailar;

/// <summary>The audio endpoints.</summary>
public sealed class AudioResource
{
    /// <summary>The transcription endpoint.</summary>
    public TranscriptionsResource Transcriptions { get; }

    /// <summary>The speech synthesis endpoint.</summary>
    public SpeechResource Speech { get; }

    internal AudioResource(RequestExecutor executor)
    {
        Transcriptions = new TranscriptionsResource(executor);
        Speech = new SpeechResource(executor);
    }
}
