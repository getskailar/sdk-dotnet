using Skailar;
using WireMock.Server;

namespace Skailar.Tests.Fixtures;

/// <summary>Boots a WireMock server for the lifetime of a test class and builds clients pointed at it.</summary>
public sealed class MockServerFixture : IDisposable
{
    public const string TestApiKey = "skl_live_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    public WireMockServer Server { get; }

    public string BaseUrl => Server.Url!;

    public MockServerFixture() => Server = WireMockServer.Start();

    /// <summary>Creates a client targeting the mock server, with retries disabled by default for deterministic tests.</summary>
    public SkailarClient CreateClient(int maxRetries = 0, TimeSpan? timeout = null) =>
        new(new SkailarClientOptions
        {
            ApiKey = TestApiKey,
            BaseUrl = BaseUrl,
            MaxRetries = maxRetries,
            Timeout = timeout ?? TimeSpan.FromSeconds(10),
        });

    public void Reset() => Server.Reset();

    /// <summary>The JSON body of the single recorded request.</summary>
    public string SingleRequestBody()
    {
        if (Server.LogEntries.Single().RequestMessage is not { } request)
        {
            throw new InvalidOperationException("No request was recorded.");
        }

        return request.Body ?? throw new InvalidOperationException("The recorded request had no body.");
    }

    /// <summary>The first value of a header on the single recorded request.</summary>
    public string SingleRequestHeader(string name)
    {
        if (Server.LogEntries.Single().RequestMessage is not { } request)
        {
            throw new InvalidOperationException("No request was recorded.");
        }

        var headers = request.Headers
            ?? throw new InvalidOperationException("The recorded request had no headers.");
        return headers[name].Single();
    }

    public void Dispose() => Server.Stop();
}
