using Skailar.Internal;

namespace Skailar;

/// <summary>The audio transcription endpoint.</summary>
public sealed class TranscriptionsResource
{
    private readonly RequestExecutor _executor;

    internal TranscriptionsResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Transcribes a base64-encoded audio clip to text.</summary>
    /// <param name="request">The transcription request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The transcription result.</returns>
    public Task<TranscriptionResponse> CreateAsync(
        TranscriptionRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendJsonAsync(
            HttpMethod.Post,
            "v1/audio/transcriptions",
            request,
            SkailarJson.Default.TranscriptionRequest,
            SkailarJson.Default.TranscriptionResponse,
            Idempotency.SideEffect,
            perCallHeaders: null,
            cancellationToken);
}
