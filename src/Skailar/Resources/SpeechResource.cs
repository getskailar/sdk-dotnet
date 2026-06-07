using Skailar.Internal;

namespace Skailar;

/// <summary>The speech synthesis endpoint.</summary>
public sealed class SpeechResource
{
    private readonly RequestExecutor _executor;

    internal SpeechResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Synthesizes speech and returns the MP3 audio as a stream.</summary>
    /// <param name="request">The speech request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>An <c>audio/mpeg</c> stream. The caller owns and must dispose it.</returns>
    public Task<Stream> CreateAsync(
        SpeechRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendForStreamAsync(
            HttpMethod.Post,
            "v1/audio/speech",
            request,
            SkailarJson.Default.SpeechRequest,
            accept: "audio/mpeg",
            cancellationToken);
}
