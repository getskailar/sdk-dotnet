namespace Skailar;

/// <summary>Classifies a <see cref="SkailarException"/> for branching on failure mode.</summary>
public enum SkailarErrorKind
{
    /// <summary>A non-2xx response that does not map to a more specific kind.</summary>
    Api,

    /// <summary>Missing, invalid, or revoked API key (HTTP 401).</summary>
    Auth,

    /// <summary>The request was malformed (HTTP 400).</summary>
    BadRequest,

    /// <summary>The requested resource does not exist (HTTP 404).</summary>
    NotFound,

    /// <summary>Rate limit exceeded (HTTP 429).</summary>
    RateLimit,

    /// <summary>An upstream model provider failed or timed out (HTTP 5xx).</summary>
    Upstream,

    /// <summary>A transport failure such as DNS, TLS, or a connection reset.</summary>
    Network,

    /// <summary>The request exceeded its configured timeout.</summary>
    Timeout,

    /// <summary>The caller cancelled the request.</summary>
    Aborted,

    /// <summary>A successful response carried a body that could not be deserialized.</summary>
    Decode,

    /// <summary>The client was misconfigured (for example, a missing API key).</summary>
    Config,
}
