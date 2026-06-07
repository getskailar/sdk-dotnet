# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.1] - Unreleased

Initial pre-release of the official Skailar .NET SDK.

### Added

- `SkailarClient` with resource groups for `Chat`, `Models`, `Images`, `Audio`, and `Uploads`,
  plus a `PingAsync` key-verification utility.
- Chat completions over JSON (`CreateAsync`) and SSE streaming
  (`CreateStreamAsync` returning `IAsyncEnumerable<ChatCompletionChunk>`).
- Model catalog (`ListAsync`, `RetrieveAsync` with slash-containing identifiers).
- Image generation, audio transcription, and speech synthesis (returning an `audio/mpeg` stream).
- Image and document uploads to Skailar storage.
- Strongly-typed request/response records with `System.Text.Json` source generators for
  trim- and NativeAOT-compatible serialization, and zero runtime dependencies.
- Full exception hierarchy (`SkailarException` and subclasses) with a `SkailarErrorKind` enum.
- `SkailarModels` constants for well-known model identifiers.

### Notes

Shipped already-corrected for bugs fixed in the sister SDKs through C++ `0.0.1`:

1. Retry backoff sleeps are interruptible via the request `CancellationToken` (`Task.Delay`,
   never `Thread.Sleep`).
2. Timeouts surface as `SkailarTimeoutException` and are distinguished from
   `SkailarConnectionException` (transport failures).
3. Streaming disposes the underlying HTTP response when enumeration completes or the consumer
   stops early (via `IAsyncEnumerable` iterator cleanup).
4. The `Authorization` header is owned by the SDK and cannot be overridden by default or
   per-call headers (case-insensitive).
5. Side-effecting `POST` requests (chat, images, audio, uploads) are never retried on `5xx`,
   preventing duplicate billing.
6. A server `Retry-After` is honored but capped at 60 seconds.
7. The SSE parser accepts all three line terminators (`\n`, `\r\n`, `\r`), including a `\r\n`
   split across reads.

[0.0.1]: https://github.com/getskailar/sdk-dotnet/releases/tag/v0.0.1
