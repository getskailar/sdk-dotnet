namespace Skailar;

/// <summary>Base type for every error raised by the Skailar SDK.</summary>
/// <remarks>
/// Catch this to handle any SDK failure. Inspect <see cref="Kind"/> to branch on the
/// failure mode, or catch a more specific subclass such as <see cref="SkailarRateLimitException"/>.
/// </remarks>
public class SkailarException : Exception
{
    /// <summary>The failure mode of this error.</summary>
    public SkailarErrorKind Kind { get; }

    /// <summary>The HTTP status code, when the error originated from a response.</summary>
    public int? Status { get; }

    /// <summary>The machine-readable error code parsed from the response body, when present.</summary>
    public string? Code { get; }

    /// <summary>The server-assigned request identifier, when the response carried one.</summary>
    public string? RequestId { get; }

    /// <summary>The raw response body, when one was read.</summary>
    public string? Raw { get; }

    /// <summary>The uncapped <c>Retry-After</c> value in seconds, set only on rate-limit errors.</summary>
    public int? RetryAfter { get; }

    /// <summary>Initializes a new instance of the <see cref="SkailarException"/> class.</summary>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="kind">The failure mode.</param>
    /// <param name="status">The HTTP status code, if any.</param>
    /// <param name="code">The machine-readable error code, if any.</param>
    /// <param name="requestId">The server-assigned request identifier, if any.</param>
    /// <param name="raw">The raw response body, if any.</param>
    /// <param name="retryAfter">The uncapped <c>Retry-After</c> value in seconds, if any.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public SkailarException(
        string message,
        SkailarErrorKind kind,
        int? status = null,
        string? code = null,
        string? requestId = null,
        string? raw = null,
        int? retryAfter = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        Status = status;
        Code = code;
        RequestId = requestId;
        Raw = raw;
        RetryAfter = retryAfter;
    }
}

/// <summary>An error returned by the Skailar API as a non-2xx HTTP response.</summary>
public class SkailarApiException : SkailarException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarApiException"/> class.</summary>
    public SkailarApiException(
        string message,
        SkailarErrorKind kind = SkailarErrorKind.Api,
        int? status = null,
        string? code = null,
        string? requestId = null,
        string? raw = null,
        int? retryAfter = null,
        Exception? innerException = null)
        : base(message, kind, status, code, requestId, raw, retryAfter, innerException)
    {
    }
}

/// <summary>The API key is missing, invalid, or revoked (HTTP 401).</summary>
public sealed class SkailarAuthException : SkailarApiException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarAuthException"/> class.</summary>
    public SkailarAuthException(
        string message,
        int? status = 401,
        string? code = null,
        string? requestId = null,
        string? raw = null)
        : base(message, SkailarErrorKind.Auth, status, code, requestId, raw)
    {
    }
}

/// <summary>The request was malformed (HTTP 400).</summary>
public sealed class SkailarBadRequestException : SkailarApiException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarBadRequestException"/> class.</summary>
    public SkailarBadRequestException(
        string message,
        int? status = 400,
        string? code = null,
        string? requestId = null,
        string? raw = null)
        : base(message, SkailarErrorKind.BadRequest, status, code, requestId, raw)
    {
    }
}

/// <summary>The requested resource does not exist (HTTP 404).</summary>
public sealed class SkailarNotFoundException : SkailarApiException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarNotFoundException"/> class.</summary>
    public SkailarNotFoundException(
        string message,
        int? status = 404,
        string? code = null,
        string? requestId = null,
        string? raw = null)
        : base(message, SkailarErrorKind.NotFound, status, code, requestId, raw)
    {
    }
}

/// <summary>The rate limit was exceeded (HTTP 429).</summary>
public sealed class SkailarRateLimitException : SkailarApiException
{
    /// <summary>The number of seconds to wait before retrying, as reported by the server.</summary>
    public int RetryAfterSeconds { get; }

    /// <summary>Initializes a new instance of the <see cref="SkailarRateLimitException"/> class.</summary>
    public SkailarRateLimitException(
        string message,
        int retryAfterSeconds,
        int? status = 429,
        string? code = null,
        string? requestId = null,
        string? raw = null)
        : base(message, SkailarErrorKind.RateLimit, status, code, requestId, raw, retryAfterSeconds)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}

/// <summary>An upstream model provider failed or timed out (HTTP 5xx).</summary>
public sealed class SkailarUpstreamException : SkailarApiException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarUpstreamException"/> class.</summary>
    public SkailarUpstreamException(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        string? raw = null)
        : base(message, SkailarErrorKind.Upstream, status, code, requestId, raw)
    {
    }
}

/// <summary>A transport-level failure prevented the request from completing.</summary>
public sealed class SkailarConnectionException : SkailarException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarConnectionException"/> class.</summary>
    public SkailarConnectionException(string message, Exception? innerException = null)
        : base(message, SkailarErrorKind.Network, innerException: innerException)
    {
    }
}

/// <summary>The request exceeded its configured timeout.</summary>
public sealed class SkailarTimeoutException : SkailarException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarTimeoutException"/> class.</summary>
    public SkailarTimeoutException(string message, Exception? innerException = null)
        : base(message, SkailarErrorKind.Timeout, innerException: innerException)
    {
    }
}

/// <summary>The client was misconfigured, for example with a missing API key.</summary>
public sealed class SkailarConfigException : SkailarException
{
    /// <summary>Initializes a new instance of the <see cref="SkailarConfigException"/> class.</summary>
    public SkailarConfigException(string message, Exception? innerException = null)
        : base(message, SkailarErrorKind.Config, innerException: innerException)
    {
    }
}
