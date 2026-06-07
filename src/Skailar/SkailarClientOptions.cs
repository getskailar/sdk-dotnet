namespace Skailar;

/// <summary>Configuration for a <see cref="SkailarClient"/>.</summary>
/// <remarks>
/// All values are read once when the client is constructed; mutating an options
/// instance afterwards has no effect on an already-created client.
/// </remarks>
public sealed class SkailarClientOptions
{
    /// <summary>
    /// The API key, of the form <c>skl_live_...</c>. When <see langword="null"/>, the client
    /// reads the <c>SKAILAR_API_KEY</c> environment variable.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The API base URL. When <see langword="null"/>, the client reads <c>SKAILAR_BASE_URL</c>
    /// and otherwise falls back to <c>https://api.skailar.com</c>. A trailing slash is trimmed.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>The per-request timeout. Defaults to 60 seconds when unset.</summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>The maximum number of retries for eligible requests. Defaults to 2.</summary>
    public int? MaxRetries { get; set; }

    /// <summary>
    /// Headers added to every request. The <c>Authorization</c> header is reserved by the SDK
    /// and any value supplied here is ignored.
    /// </summary>
    public IDictionary<string, string> DefaultHeaders { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// An existing <see cref="System.Net.Http.HttpClient"/> to reuse, for connection pooling or
    /// dependency injection. When supplied, the <see cref="SkailarClient"/> does not dispose it.
    /// When <see langword="null"/>, the client creates and owns its own instance.
    /// </summary>
    public HttpClient? HttpClient { get; set; }
}
