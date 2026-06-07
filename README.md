# Skailar .NET SDK

The official .NET SDK for the [Skailar](https://skailar.com) API — a multi-provider LLM
gateway with an OpenAI-compatible surface. One API key and one wire format reach Claude,
GPT, Gemini, DeepSeek, Grok, and more, with chat completions, model discovery, image
generation, speech synthesis, transcription, and storage uploads.

## Installation

```sh
dotnet add package Skailar
```

Target frameworks: `net8.0` and `net9.0`. Zero runtime dependencies — everything is built on
`System.Net.Http` and `System.Text.Json`. The library is trim- and NativeAOT-compatible.

## Quickstart

```csharp
using Skailar;

// Reads SKAILAR_API_KEY from the environment; base URL defaults to https://api.skailar.com.
using var client = new SkailarClient();

ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages =
    [
        ChatMessage.User("Hello!"),
    ],
});

Console.WriteLine(response.Choices[0].Message.Content?.GetText());
```

API keys are bearer tokens of the form `skl_live_…`, created from the
[Skailar dashboard](https://skailar.com).

## Streaming

`CreateStreamAsync` returns an `IAsyncEnumerable<ChatCompletionChunk>`. Breaking out of the
loop disposes the underlying response, and a `CancellationToken` stops the stream at any point.

```csharp
var request = new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages = [ChatMessage.User("Write a haiku about the sea.")],
};

await foreach (ChatCompletionChunk chunk in client.Chat.Completions.CreateStreamAsync(request))
{
    if (chunk.Choices[0].Delta.Content is { } text)
    {
        Console.Write(text);
    }
}
```

## Configuration

`SkailarClientOptions` configures the client. All values are read once at construction; the
client is immutable and thread-safe afterwards.

```csharp
using var client = new SkailarClient(new SkailarClientOptions
{
    ApiKey = "skl_live_...",
    BaseUrl = "http://localhost:8080",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetries = 2,
    DefaultHeaders =
    {
        ["X-Trace-Id"] = "abc123",
    },
});
```

| Option | Default | Notes |
| --- | --- | --- |
| `ApiKey` | `SKAILAR_API_KEY` env var | Required (option or environment). |
| `BaseUrl` | `SKAILAR_BASE_URL` env var, then `https://api.skailar.com` | Trailing slash trimmed. |
| `Timeout` | 60 seconds | Per-request, distinguishes timeout from connection failure. |
| `MaxRetries` | 2 | Applies to eligible requests only (see Retries). |
| `DefaultHeaders` | empty | `Authorization` is reserved and cannot be overridden. |
| `HttpClient` | created internally | Supply your own for pooling/DI; then you own its disposal. |

## Dependency injection

The client is a plain object and integrates cleanly with `Microsoft.Extensions.DependencyInjection`,
though the SDK does not depend on it.

```csharp
// Reuse a pooled HttpClient from IHttpClientFactory:
services.AddHttpClient();
services.AddSingleton(sp => new SkailarClient(new SkailarClientOptions
{
    ApiKey = config["Skailar:ApiKey"],
    HttpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
}));

// Or register the typed client directly:
services.AddHttpClient<SkailarClient>();
```

When you pass your own `HttpClient`, the SDK never disposes it — its lifetime is yours.

## Drop-in OpenAI alternative

The wire format for chat completions, models, image generation, transcription, and speech
mirrors the OpenAI API. Existing OpenAI integrations can point at Skailar by overriding the
base URL to `https://api.skailar.com/v1` — or migrate to this SDK and use the strongly-typed
Skailar models directly. Field names, error shapes, and streaming framing are all OpenAI-compatible.

## Error handling

Every failure is a `SkailarException`. Catch a specific subclass for targeted handling, or
inspect `Kind` to branch with pattern matching.

```csharp
try
{
    await client.Chat.Completions.CreateAsync(request);
}
catch (SkailarRateLimitException ex)
{
    await Task.Delay(TimeSpan.FromSeconds(ex.RetryAfterSeconds));
}
catch (SkailarApiException ex) when (ex.Status == 503)
{
    // Upstream provider unavailable.
}
catch (SkailarException ex)
{
    Console.Error.WriteLine($"{ex.Kind}: {ex.Message} (request {ex.RequestId})");
}
```

| Exception | When |
| --- | --- |
| `SkailarAuthException` | 401 — missing, invalid, or revoked key. |
| `SkailarBadRequestException` | 400 — malformed request. |
| `SkailarNotFoundException` | 404 — unknown resource. |
| `SkailarRateLimitException` | 429 — carries `RetryAfterSeconds`. |
| `SkailarUpstreamException` | 5xx — upstream provider failed. |
| `SkailarApiException` | any other non-2xx response. |
| `SkailarConnectionException` | transport failure (DNS, TLS, reset). |
| `SkailarTimeoutException` | request exceeded its timeout. |
| `SkailarConfigException` | misconfiguration (for example, missing key). |

### Retries

Eligible requests are retried with exponential backoff and full jitter (base 500 ms, capped at
8 s), honoring a server `Retry-After` header up to 60 seconds. Sleeps are cancellable via the
request's `CancellationToken`.

- `429` is always retried.
- `5xx` and transient transport failures are retried only for idempotent requests
  (`Models.ListAsync`, `Models.RetrieveAsync`, `PingAsync`).
- Side-effecting `POST`s (chat completions, images, audio, uploads) are **not** retried on
  `5xx`, to avoid duplicate billing.

## Cancellation

Every asynchronous method accepts a trailing `CancellationToken`. It cancels in-flight requests,
interrupts retry backoff, and stops streaming enumeration.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var response = await client.Chat.Completions.CreateAsync(request, cts.Token);
```

## Local development

```sh
dotnet build -c Release      # builds net8.0 + net9.0 with warnings as errors
dotnet test                  # runs the xUnit + WireMock.Net suite
dotnet pack -c Release -o ./artifacts   # produces Skailar.0.0.1.nupkg (+ .snupkg)
```

Run a sample against a live gateway:

```sh
SKAILAR_API_KEY=skl_live_... SKAILAR_BASE_URL=http://localhost:8080 \
  dotnet run --project samples/Chat
```

## Status

Pre-release `0.0.1`. The public surface may still change before `1.0`.

## License

MIT. See [LICENSE](LICENSE).
