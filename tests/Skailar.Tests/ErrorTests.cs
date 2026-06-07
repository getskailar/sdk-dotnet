using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class ErrorTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    private void StubPing(int status, string body, Action<IResponseBuilder>? customize = null)
    {
        _fixture.Reset();
        IResponseBuilder response = Response.Create().WithStatusCode(status).WithBody(body);
        customize?.Invoke(response);
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/ping-key").UsingGet())
            .RespondWith(response);
    }

    [Fact]
    public async Task Status401_ThrowsAuthException()
    {
        StubPing(401, Samples.Error("invalid_api_key", "Invalid key"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarAuthException ex = await Assert.ThrowsAsync<SkailarAuthException>(() => client.PingAsync());
        Assert.Equal(SkailarErrorKind.Auth, ex.Kind);
        Assert.Equal(401, ex.Status);
        Assert.Equal("invalid_api_key", ex.Code);
        Assert.Equal("Invalid key", ex.Message);
    }

    [Fact]
    public async Task Status400_ThrowsBadRequestException()
    {
        StubPing(400, Samples.Error("bad_request", "Malformed"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarBadRequestException ex = await Assert.ThrowsAsync<SkailarBadRequestException>(() => client.PingAsync());
        Assert.Equal(SkailarErrorKind.BadRequest, ex.Kind);
    }

    [Fact]
    public async Task Status404_ThrowsNotFoundException()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models/ghost").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404).WithBody(Samples.Error("not_found", "No such model")));
        using SkailarClient client = _fixture.CreateClient();

        SkailarNotFoundException ex = await Assert.ThrowsAsync<SkailarNotFoundException>(() => client.Models.RetrieveAsync("ghost"));
        Assert.Equal(SkailarErrorKind.NotFound, ex.Kind);
    }

    [Fact]
    public async Task Status503_ThrowsUpstreamException()
    {
        StubPing(503, Samples.Error("upstream_error", "Provider down"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarUpstreamException ex = await Assert.ThrowsAsync<SkailarUpstreamException>(() => client.PingAsync());
        Assert.Equal(SkailarErrorKind.Upstream, ex.Kind);
        Assert.Equal(503, ex.Status);
    }

    [Fact]
    public async Task UpstreamException_IsCatchableAsApiException()
    {
        StubPing(502, Samples.Error("upstream_error", "Bad gateway"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarApiException ex = await Assert.ThrowsAsync<SkailarUpstreamException>(() => client.PingAsync());
        Assert.Equal(502, ex.Status);
    }

    [Fact]
    public async Task Error_ExtractsRequestIdFromHeader()
    {
        StubPing(401, Samples.Error("invalid_api_key", "nope"),
            r => r.WithHeader("X-Request-Id", "req_abc123"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarException ex = await Assert.ThrowsAsync<SkailarAuthException>(() => client.PingAsync());
        Assert.Equal("req_abc123", ex.RequestId);
    }

    [Fact]
    public async Task Error_FallsBackToSkailarRequestIdHeader()
    {
        StubPing(401, Samples.Error("invalid_api_key", "nope"),
            r => r.WithHeader("X-Skailar-Request-Id", "skl_req_9"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarException ex = await Assert.ThrowsAsync<SkailarAuthException>(() => client.PingAsync());
        Assert.Equal("skl_req_9", ex.RequestId);
    }

    [Fact]
    public async Task Error_RawBodyPreserved()
    {
        string body = Samples.Error("invalid_api_key", "nope");
        StubPing(401, body);
        using SkailarClient client = _fixture.CreateClient();

        SkailarException ex = await Assert.ThrowsAsync<SkailarAuthException>(() => client.PingAsync());
        Assert.Contains("invalid_api_key", ex.Raw);
    }

    [Fact]
    public async Task Error_UnmappedStatus_ThrowsGenericApiException()
    {
        StubPing(418, Samples.Error("teapot", "short and stout"));
        using SkailarClient client = _fixture.CreateClient();

        SkailarApiException ex = await Assert.ThrowsAsync<SkailarApiException>(() => client.PingAsync());
        Assert.Equal(SkailarErrorKind.Api, ex.Kind);
        Assert.Equal(418, ex.Status);
    }
}
