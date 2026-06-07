using System.Text;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class StreamingTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    private void StubStream(string body) =>
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/event-stream")
                .WithBody(body));

    private static ChatCompletionRequest Request_() => new()
    {
        Model = "claude-sonnet-4-6",
        Messages = [ChatMessage.User("hi")],
    };

    [Fact]
    public async Task CreateStreamAsync_YieldsAllChunks()
    {
        _fixture.Reset();
        StubStream(Samples.Streaming("Hel", "lo", "!"));

        using SkailarClient client = _fixture.CreateClient();
        var text = new StringBuilder();
        await foreach (ChatCompletionChunk chunk in client.Chat.Completions.CreateStreamAsync(Request_()))
        {
            if (chunk.Choices[0].Delta.Content is { } piece)
            {
                text.Append(piece);
            }
        }

        Assert.Equal("Hello!", text.ToString());
    }

    [Fact]
    public async Task CreateStreamAsync_StopsAtDoneSentinel()
    {
        _fixture.Reset();
        StubStream(Samples.Streaming("a", "b"));

        using SkailarClient client = _fixture.CreateClient();
        int count = 0;
        await foreach (ChatCompletionChunk _ in client.Chat.Completions.CreateStreamAsync(Request_()))
        {
            count++;
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CreateStreamAsync_EarlyBreak_StopsEnumeration()
    {
        _fixture.Reset();
        StubStream(Samples.Streaming("one", "two", "three", "four"));

        using SkailarClient client = _fixture.CreateClient();
        var seen = new List<string>();
        await foreach (ChatCompletionChunk chunk in client.Chat.Completions.CreateStreamAsync(Request_()))
        {
            seen.Add(chunk.Choices[0].Delta.Content!);
            if (seen.Count == 2)
            {
                break;
            }
        }

        Assert.Equal(["one", "two"], seen);
    }

    [Fact]
    public async Task CreateStreamAsync_HonorsCancellationToken()
    {
        _fixture.Reset();
        StubStream(Samples.Streaming("x", "y", "z"));

        using SkailarClient client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (ChatCompletionChunk _ in client.Chat.Completions.CreateStreamAsync(Request_(), cts.Token))
            {
                await cts.CancelAsync();
            }
        });
    }

    [Fact]
    public async Task CreateStreamAsync_InBandError_ThrowsDuringIteration()
    {
        _fixture.Reset();
        string body = "data: {\"error\":{\"type\":\"upstream_error\",\"message\":\"mid-stream boom\"}}\n\n";
        StubStream(body);

        using SkailarClient client = _fixture.CreateClient();

        SkailarException ex = await Assert.ThrowsAsync<SkailarUpstreamException>(async () =>
        {
            await foreach (ChatCompletionChunk _ in client.Chat.Completions.CreateStreamAsync(Request_()))
            {
            }
        });

        Assert.Contains("mid-stream boom", ex.Message);
    }

    [Fact]
    public async Task CreateStreamAsync_SetsStreamTrueInRequest()
    {
        _fixture.Reset();
        StubStream(Samples.Streaming("hi"));

        using SkailarClient client = _fixture.CreateClient();
        await foreach (ChatCompletionChunk _ in client.Chat.Completions.CreateStreamAsync(Request_()))
        {
        }

        using var body = System.Text.Json.JsonDocument.Parse(_fixture.SingleRequestBody());
        Assert.True(body.RootElement.GetProperty("stream").GetBoolean());
    }
}
