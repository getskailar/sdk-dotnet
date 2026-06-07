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

    public void Dispose() => Server.Stop();
}
