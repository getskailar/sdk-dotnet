using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Skailar.Internal;

/// <summary>Dispatches HTTP requests with authentication, retry, and error mapping.</summary>
internal sealed class RequestExecutor
{
    private const int BackoffBaseMs = 500;
    private const int BackoffCapMs = 8_000;
    private const int MaxRetryAfterSeconds = 60;
    private const int BackoffMaxShift = 20;

    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly int _maxRetries;
    private readonly TimeSpan _timeout;
    private readonly IReadOnlyDictionary<string, string> _defaultHeaders;

    public RequestExecutor(HttpClient http, string baseUrl, string apiKey, int maxRetries, TimeSpan? timeout, IReadOnlyDictionary<string, string> defaultHeaders)
    {
        _http = http;
        _baseUrl = baseUrl.TrimEnd('/');
        _apiKey = apiKey;
        _maxRetries = maxRetries;
        _timeout = timeout ?? DefaultTimeout;
        _defaultHeaders = defaultHeaders;
    }

    /// <summary>Sends a JSON request body and deserializes the JSON response.</summary>
    public async Task<TResponse> SendJsonAsync<TRequest, TResponse>(
        HttpMethod method,
        string path,
        TRequest body,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo,
        Idempotency idempotency,
        IReadOnlyDictionary<string, string>? perCallHeaders = null,
        CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(body, requestTypeInfo);
        using HttpResponseMessage response = await SendWithRetryAsync(
            method, path, idempotency, perCallHeaders, accept: "application/json",
            () => JsonContent(payload), cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync(response, responseTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Sends a request without a body and deserializes the JSON response.</summary>
    public async Task<TResponse> GetJsonAsync<TResponse>(
        string path,
        JsonTypeInfo<TResponse> responseTypeInfo,
        Idempotency idempotency = Idempotency.Idempotent,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await SendWithRetryAsync(
            HttpMethod.Get, path, idempotency, perCallHeaders: null, accept: "application/json",
            contentFactory: null, cancellationToken).ConfigureAwait(false);

        return await DeserializeAsync(response, responseTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Sends a JSON request body and returns the raw response stream (for binary responses such as audio).</summary>
    public async Task<Stream> SendForStreamAsync<TRequest>(
        HttpMethod method,
        string path,
        TRequest body,
        JsonTypeInfo<TRequest> requestTypeInfo,
        string accept,
        CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(body, requestTypeInfo);

        // Side-effecting binary responses are never retried; a single attempt keeps the live stream open.
        HttpResponseMessage response = await SendOnceAsync(
            method, path, perCallHeaders: null, accept, () => JsonContent(payload),
            HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowFromResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Opens an SSE stream for the given JSON request body. The response is not retried.</summary>
    public async Task<HttpResponseMessage> OpenSseAsync<TRequest>(
        string path,
        TRequest body,
        JsonTypeInfo<TRequest> requestTypeInfo,
        CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(body, requestTypeInfo);

        HttpResponseMessage response = await SendOnceAsync(
            HttpMethod.Post, path, perCallHeaders: null, accept: "text/event-stream",
            () => JsonContent(payload), HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowFromResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private static ByteArrayContent JsonContent(byte[] payload)
    {
        var content = new ByteArrayContent(payload);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
        return content;
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync(
        HttpMethod method,
        string path,
        Idempotency idempotency,
        IReadOnlyDictionary<string, string>? perCallHeaders,
        string accept,
        Func<HttpContent>? contentFactory,
        CancellationToken cancellationToken)
    {
        int maxAttempts = _maxRetries + 1;
        for (int attempt = 0; ; attempt++)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await SendOnceAsync(
                    method, path, perCallHeaders, accept, contentFactory,
                    HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                int status = (int)response.StatusCode;
                if (ShouldRetryStatus(status, idempotency, attempt, maxAttempts))
                {
                    TimeSpan delay = RetryDelay(response, attempt);
                    response.Dispose();
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                await ThrowFromResponseAsync(response, cancellationToken).ConfigureAwait(false);
            }
            catch (SkailarException)
            {
                response?.Dispose();
                throw;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                response?.Dispose();
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or IOException)
            {
                response?.Dispose();
                bool transient = idempotency == Idempotency.Idempotent;
                if (transient && attempt + 1 < maxAttempts)
                {
                    await Task.Delay(RetryDelay(response: null, attempt), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                throw Translate(ex);
            }
        }
    }

    private async Task<HttpResponseMessage> SendOnceAsync(
        HttpMethod method,
        string path,
        IReadOnlyDictionary<string, string>? perCallHeaders,
        string accept,
        Func<HttpContent>? contentFactory,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, Endpoint(path));
        if (contentFactory is not null)
        {
            request.Content = contentFactory();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Headers.Accept.ParseAdd(accept);

        foreach (var (key, value) in HeaderUtils.Merge(_defaultHeaders, perCallHeaders))
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_timeout);

        try
        {
            return await _http.SendAsync(request, completionOption, timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && timeoutCts.IsCancellationRequested)
        {
            throw new SkailarTimeoutException($"The request to '{path}' timed out after {_timeout.TotalSeconds:0.#}s.");
        }
    }

    private string Endpoint(string path) => $"{_baseUrl}/{path.TrimStart('/')}";

    private static bool ShouldRetryStatus(int status, Idempotency idempotency, int attempt, int maxAttempts)
    {
        if (attempt + 1 >= maxAttempts)
        {
            return false;
        }

        if (status == 429)
        {
            return true;
        }

        return status >= 500 && idempotency == Idempotency.Idempotent;
    }

    private static TimeSpan RetryDelay(HttpResponseMessage? response, int attempt)
    {
        if (response is not null && TryGetRetryAfterSeconds(response, out int retryAfter))
        {
            int capped = Math.Min(retryAfter, MaxRetryAfterSeconds);
            return TimeSpan.FromSeconds(Math.Max(0, capped));
        }

        int shift = Math.Min(attempt, BackoffMaxShift);
        long window = Math.Min(BackoffCapMs, BackoffBaseMs * (1L << shift));
        double jittered = Random.Shared.NextDouble() * window;
        return TimeSpan.FromMilliseconds(jittered);
    }

    private static bool TryGetRetryAfterSeconds(HttpResponseMessage response, out int seconds)
    {
        seconds = 0;
        RetryConditionHeaderValue? retryAfter = response.Headers.RetryAfter;
        if (retryAfter is null)
        {
            return false;
        }

        if (retryAfter.Delta is { } delta)
        {
            seconds = (int)Math.Ceiling(delta.TotalSeconds);
            return true;
        }

        if (retryAfter.Date is { } date)
        {
            double diff = (date - DateTimeOffset.UtcNow).TotalSeconds;
            seconds = (int)Math.Ceiling(Math.Max(0, diff));
            return true;
        }

        return false;
    }

    private async Task<TResponse> DeserializeAsync<TResponse>(
        HttpResponseMessage response,
        JsonTypeInfo<TResponse> typeInfo,
        CancellationToken cancellationToken)
    {
        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            TResponse? value = await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken).ConfigureAwait(false);
            return value ?? throw new SkailarException("The server returned an empty response body.", SkailarErrorKind.Decode);
        }
        catch (JsonException ex)
        {
            throw new SkailarException($"Failed to decode the response body: {ex.Message}", SkailarErrorKind.Decode, innerException: ex);
        }
    }

    private static async Task ThrowFromResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        int status = (int)response.StatusCode;
        string raw = await SafeReadBodyAsync(response, cancellationToken).ConfigureAwait(false);
        (string? code, string? message) = ErrorBody.Parse(raw);
        string? requestId = ExtractRequestId(response);
        string text = message ?? response.ReasonPhrase ?? $"HTTP {status}";

        throw status switch
        {
            401 => new SkailarAuthException(text, status, code, requestId, raw),
            400 => new SkailarBadRequestException(text, status, code, requestId, raw),
            404 => new SkailarNotFoundException(text, status, code, requestId, raw),
            429 => new SkailarRateLimitException(text, RateLimitRetryAfter(response), status, code, requestId, raw),
            >= 500 => new SkailarUpstreamException(text, status, code, requestId, raw),
            _ => new SkailarApiException(text, SkailarErrorKind.Api, status, code, requestId, raw),
        };
    }

    private static int RateLimitRetryAfter(HttpResponseMessage response) =>
        TryGetRetryAfterSeconds(response, out int seconds) ? Math.Min(seconds, MaxRetryAfterSeconds) : 0;

    private static async Task<string> SafeReadBodyAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException or OperationCanceledException)
        {
            return string.Empty;
        }
    }

    private static string? ExtractRequestId(HttpResponseMessage response)
    {
        foreach (string name in (ReadOnlySpan<string>)["x-request-id", "x-skailar-request-id", "request-id"])
        {
            if (response.Headers.TryGetValues(name, out IEnumerable<string>? values))
            {
                string? value = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
        }

        return null;
    }

    private static SkailarException Translate(Exception ex) => ex switch
    {
        TaskCanceledException => new SkailarTimeoutException("The request timed out.", ex),
        HttpRequestException http => new SkailarConnectionException(http.Message, http),
        IOException io => new SkailarConnectionException(io.Message, io),
        _ => new SkailarConnectionException(ex.Message, ex),
    };
}
