using System.Diagnostics;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class RetryTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task Get5xx_RetriesAndSucceeds()
    {
        _fixture.Reset();
        const string scenario = "models-5xx";
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models").UsingGet())
            .InScenario(scenario).WillSetStateTo("second")
            .RespondWith(Response.Create().WithStatusCode(503).WithBody(Samples.Error("upstream_error", "down")));
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models").UsingGet())
            .InScenario(scenario).WhenStateIs("second")
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.ModelList()));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 2);
        ModelList list = await client.Models.ListAsync();

        Assert.Single(list.Data);
        Assert.Equal(2, _fixture.Server.LogEntries.Count());
    }

    [Fact]
    public async Task Post5xx_DoesNotRetry()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(500).WithBody(Samples.Error("upstream_error", "boom")));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 3);
        await Assert.ThrowsAsync<SkailarUpstreamException>(() => client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
        }));

        Assert.Single(_fixture.Server.LogEntries);
    }

    [Fact]
    public async Task Post429_RetriesEvenForSideEffect()
    {
        _fixture.Reset();
        const string scenario = "chat-429";
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .InScenario(scenario).WillSetStateTo("ok")
            .RespondWith(Response.Create().WithStatusCode(429).WithHeader("Retry-After", "0").WithBody(Samples.Error("rate_limited", "slow down")));
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .InScenario(scenario).WhenStateIs("ok")
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("recovered")));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 2);
        ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
        });

        Assert.Equal("recovered", response.Choices[0].Message.Content!.GetText());
        Assert.Equal(2, _fixture.Server.LogEntries.Count());
    }

    [Fact]
    public async Task Get429_ExhaustsRetries_ThrowsRateLimit()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(429).WithHeader("Retry-After", "0").WithBody(Samples.Error("rate_limited", "no")));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 1);
        SkailarRateLimitException ex = await Assert.ThrowsAsync<SkailarRateLimitException>(() => client.PingAsync());

        Assert.Equal(2, _fixture.Server.LogEntries.Count());
        Assert.Equal(0, ex.RetryAfterSeconds);
    }

    [Fact]
    public async Task RateLimit_RetryAfterCappedAtSixtySeconds()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(429).WithHeader("Retry-After", "3600").WithBody(Samples.Error("rate_limited", "later")));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 0);
        SkailarRateLimitException ex = await Assert.ThrowsAsync<SkailarRateLimitException>(() => client.PingAsync());

        Assert.Equal(60, ex.RetryAfterSeconds);
    }

    [Fact]
    public async Task RetryAfter_DelayIsHonored()
    {
        _fixture.Reset();
        const string scenario = "retry-after-delay";
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .InScenario(scenario).WillSetStateTo("ok")
            .RespondWith(Response.Create().WithStatusCode(429).WithHeader("Retry-After", "1").WithBody(Samples.Error("rate_limited", "wait")));
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .InScenario(scenario).WhenStateIs("ok")
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "status": "ok", "user_id": "u1" }"""));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 2);
        var sw = Stopwatch.StartNew();
        PingKeyResponse response = await client.PingAsync();
        sw.Stop();

        Assert.Equal("ok", response.Status);
        Assert.True(sw.ElapsedMilliseconds >= 900, $"expected >= ~1s delay, got {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task Get4xxNon429_NeverRetries()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(401).WithBody(Samples.Error("invalid_api_key", "no")));

        using SkailarClient client = _fixture.CreateClient(maxRetries: 5);
        await Assert.ThrowsAsync<SkailarAuthException>(() => client.PingAsync());

        Assert.Single(_fixture.Server.LogEntries);
    }
}
