using Skailar.Internal;

namespace Skailar;

/// <summary>
/// The entry point to the Skailar API. Create one instance and reuse it; the client is
/// safe for concurrent use from multiple threads, and its configuration is immutable after
/// construction.
/// </summary>
/// <example>
/// <code>
/// using var client = new SkailarClient();
/// var response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
/// {
///     Model = SkailarModels.ClaudeSonnet4_6,
///     Messages = [ChatMessage.User("Hello!")],
/// });
/// Console.WriteLine(response.Choices[0].Message.Content?.GetText());
/// </code>
/// </example>
public sealed class SkailarClient : IDisposable
{
    private const string DefaultBaseUrl = "https://api.skailar.com";
    private const int DefaultMaxRetries = 2;
    private const string ApiKeyEnvVar = "SKAILAR_API_KEY";
    private const string BaseUrlEnvVar = "SKAILAR_BASE_URL";

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    /// <summary>The chat endpoints.</summary>
    public ChatResource Chat { get; }

    /// <summary>The model-catalog endpoints.</summary>
    public ModelsResource Models { get; }

    /// <summary>The image generation endpoints.</summary>
    public ImagesResource Images { get; }

    /// <summary>The audio endpoints.</summary>
    public AudioResource Audio { get; }

    /// <summary>The storage upload endpoints.</summary>
    public UploadsResource Uploads { get; }

    private readonly RequestExecutor _executor;

    /// <summary>Creates a client using the default options, reading <c>SKAILAR_API_KEY</c> from the environment.</summary>
    /// <exception cref="SkailarConfigException">No API key was supplied or found in the environment.</exception>
    public SkailarClient()
        : this(new SkailarClientOptions())
    {
    }

    /// <summary>Creates a client with the given options.</summary>
    /// <param name="options">The client configuration.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="SkailarConfigException">No API key was supplied or found in the environment.</exception>
    public SkailarClient(SkailarClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string apiKey = ResolveApiKey(options.ApiKey);
        string baseUrl = ResolveBaseUrl(options.BaseUrl);
        int maxRetries = options.MaxRetries is { } retries && retries >= 0 ? retries : DefaultMaxRetries;

        if (options.HttpClient is { } provided)
        {
            _http = provided;
            _ownsHttpClient = false;
        }
        else
        {
            _http = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
            _ownsHttpClient = true;
        }

        var headers = new Dictionary<string, string>(options.DefaultHeaders, StringComparer.OrdinalIgnoreCase);

        _executor = new RequestExecutor(_http, baseUrl, apiKey, maxRetries, options.Timeout, headers);

        Chat = new ChatResource(_executor);
        Models = new ModelsResource(_executor);
        Images = new ImagesResource(_executor);
        Audio = new AudioResource(_executor);
        Uploads = new UploadsResource(_executor);
    }

    /// <summary>Verifies the configured API key.</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The key status and owning user id.</returns>
    /// <exception cref="SkailarAuthException">The key is missing, invalid, or revoked.</exception>
    public Task<PingKeyResponse> PingAsync(CancellationToken cancellationToken = default) =>
        _executor.GetJsonAsync("v1/ping-key", SkailarJson.Default.PingKeyResponse, Idempotency.Idempotent, cancellationToken);

    private static string ResolveApiKey(string? explicitKey)
    {
        string? key = explicitKey ?? Environment.GetEnvironmentVariable(ApiKeyEnvVar);
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SkailarConfigException(
                $"No API key supplied. Set the {ApiKeyEnvVar} environment variable or pass SkailarClientOptions.ApiKey.");
        }

        return key;
    }

    private static string ResolveBaseUrl(string? explicitUrl)
    {
        string? url = explicitUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            url = Environment.GetEnvironmentVariable(BaseUrlEnvVar);
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            url = DefaultBaseUrl;
        }

        return url.TrimEnd('/');
    }

    /// <summary>Disposes the underlying <see cref="HttpClient"/> when this client created it.</summary>
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }
}
