using System.Runtime.CompilerServices;
using Skailar.Internal;

namespace Skailar;

/// <summary>Creates chat completions, with or without streaming.</summary>
public sealed class ChatCompletionsResource
{
    private readonly RequestExecutor _executor;

    internal ChatCompletionsResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Creates a chat completion and returns the full response.</summary>
    /// <param name="request">The completion request. Leave <see cref="ChatCompletionRequest.Stream"/> unset or <see langword="false"/>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generated completion.</returns>
    /// <exception cref="SkailarException">The request failed. Catch a subclass such as <see cref="SkailarRateLimitException"/> for specific handling.</exception>
    public Task<ChatCompletionResponse> CreateAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        ChatCompletionRequest body = request.Stream is true ? request with { Stream = false } : request;
        return _executor.SendJsonAsync(
            HttpMethod.Post,
            "v1/chat/completions",
            body,
            SkailarJson.Default.ChatCompletionRequest,
            SkailarJson.Default.ChatCompletionResponse,
            Idempotency.SideEffect,
            perCallHeaders: null,
            cancellationToken);
    }

    /// <summary>Creates a streaming chat completion and yields incremental chunks.</summary>
    /// <param name="request">The completion request. <see cref="ChatCompletionRequest.Stream"/> is forced to <see langword="true"/>.</param>
    /// <param name="cancellationToken">A token to stop the stream; breaking out of the loop also disposes it.</param>
    /// <returns>An asynchronous sequence of completion chunks.</returns>
    /// <exception cref="SkailarException">The request failed, or an error frame arrived mid-stream.</exception>
    public async IAsyncEnumerable<ChatCompletionChunk> CreateStreamAsync(
        ChatCompletionRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ChatCompletionRequest body = request.Stream is true ? request : request with { Stream = true };
        HttpResponseMessage response = await _executor
            .OpenSseAsync("v1/chat/completions", body, SkailarJson.Default.ChatCompletionRequest, cancellationToken)
            .ConfigureAwait(false);

        await foreach (ChatCompletionChunk chunk in ChatCompletionStream.ReadAsync(response, cancellationToken).ConfigureAwait(false))
        {
            yield return chunk;
        }
    }
}
