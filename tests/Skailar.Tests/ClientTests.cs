using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class ClientTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public void Constructor_MissingApiKey_ThrowsConfigException()
    {
        string? previous = Environment.GetEnvironmentVariable("SKAILAR_API_KEY");
        try
        {
            Environment.SetEnvironmentVariable("SKAILAR_API_KEY", null);
            SkailarConfigException ex = Assert.Throws<SkailarConfigException>(() => new SkailarClient());
            Assert.Equal(SkailarErrorKind.Config, ex.Kind);
        }
        finally
        {
            Environment.SetEnvironmentVariable("SKAILAR_API_KEY", previous);
        }
    }

    [Fact]
    public async Task Constructor_ReadsApiKeyFromEnvironment()
    {
        string? previousKey = Environment.GetEnvironmentVariable("SKAILAR_API_KEY");
        string? previousUrl = Environment.GetEnvironmentVariable("SKAILAR_BASE_URL");
        try
        {
            _fixture.Reset();
            _fixture.Server
                .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "status": "ok", "user_id": "env-user" }"""));

            Environment.SetEnvironmentVariable("SKAILAR_API_KEY", MockServerFixture.TestApiKey);
            Environment.SetEnvironmentVariable("SKAILAR_BASE_URL", _fixture.BaseUrl);

            using var client = new SkailarClient();
            PingKeyResponse response = await client.PingAsync();
            Assert.Equal("env-user", response.UserId);
        }
        finally
        {
            Environment.SetEnvironmentVariable("SKAILAR_API_KEY", previousKey);
            Environment.SetEnvironmentVariable("SKAILAR_BASE_URL", previousUrl);
        }
    }

    [Fact]
    public async Task DefaultHeaders_AreSentOnEveryRequest()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "status": "ok", "user_id": "u" }"""));

        using var client = new SkailarClient(new SkailarClientOptions
        {
            ApiKey = MockServerFixture.TestApiKey,
            BaseUrl = _fixture.BaseUrl,
            DefaultHeaders = { ["X-Trace-Id"] = "trace-42" },
        });
        await client.PingAsync();

        string trace = _fixture.Server.LogEntries.Single().RequestMessage.Headers!["X-Trace-Id"].Single();
        Assert.Equal("trace-42", trace);
    }

    [Fact]
    public async Task DefaultHeaders_CannotOverrideAuthorization()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "status": "ok", "user_id": "u" }"""));

        using var client = new SkailarClient(new SkailarClientOptions
        {
            ApiKey = MockServerFixture.TestApiKey,
            BaseUrl = _fixture.BaseUrl,
            DefaultHeaders = { ["Authorization"] = "Bearer hacker", ["authorization"] = "Bearer hacker2" },
        });
        await client.PingAsync();

        string auth = _fixture.Server.LogEntries.Single().RequestMessage.Headers!["Authorization"].Single();
        Assert.Equal($"Bearer {MockServerFixture.TestApiKey}", auth);
    }

    [Fact]
    public async Task ProvidedHttpClient_IsNotDisposedByClient()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "status": "ok", "user_id": "u" }"""));

        using var http = new HttpClient();
        var client = new SkailarClient(new SkailarClientOptions
        {
            ApiKey = MockServerFixture.TestApiKey,
            BaseUrl = _fixture.BaseUrl,
            HttpClient = http,
        });

        client.Dispose();

        PingKeyResponse response = await client.PingAsync();
        Assert.Equal("u", response.UserId);
    }

    [Fact]
    public void BaseUrl_TrailingSlashIsTrimmed()
    {
        using var client = new SkailarClient(new SkailarClientOptions
        {
            ApiKey = MockServerFixture.TestApiKey,
            BaseUrl = "https://example.com/",
        });

        Assert.NotNull(client);
    }
}
