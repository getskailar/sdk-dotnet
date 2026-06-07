using System.Text.Json;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class ChatTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task CreateAsync_ReturnsCompletion()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("Hi there!")));

        using SkailarClient client = _fixture.CreateClient();
        ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = SkailarModels.ClaudeSonnet4_6,
            Messages = [ChatMessage.User("hi")],
        });

        Assert.Equal("Hi there!", response.Choices[0].Message.Content!.GetText());
        Assert.Equal(FinishReason.Stop, response.Choices[0].FinishReason);
        Assert.Equal(12, response.Usage!.TotalTokens);
    }

    [Fact]
    public async Task CreateAsync_SendsBearerToken()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
        });

        var log = _fixture.Server.LogEntries.Single();
        string auth = log.RequestMessage.Headers!["Authorization"].Single();
        Assert.Equal($"Bearer {MockServerFixture.TestApiKey}", auth);
    }

    [Fact]
    public async Task CreateAsync_SerializesRequestAsSnakeCase()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
            MaxTokens = 64,
            ReasoningEffort = ReasoningEffort.High,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        JsonElement root = body.RootElement;
        Assert.Equal(64, root.GetProperty("max_tokens").GetInt32());
        Assert.Equal("high", root.GetProperty("reasoning_effort").GetString());
        Assert.Equal("user", root.GetProperty("messages")[0].GetProperty("role").GetString());
    }

    [Fact]
    public async Task CreateAsync_OmitsNullOptionalFields()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.False(body.RootElement.TryGetProperty("temperature", out _));
        Assert.False(body.RootElement.TryGetProperty("tools", out _));
    }

    [Fact]
    public async Task CreateAsync_NonStreaming_ForcesStreamFalse()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
            Stream = true,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.False(body.RootElement.GetProperty("stream").GetBoolean());
    }

    [Fact]
    public async Task CreateAsync_SendsVisionParts()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages =
            [
                ChatMessage.User([ContentPart.Text("What is this?"), ContentPart.Image("https://example.com/a.png", "high")]),
            ],
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        JsonElement content = body.RootElement.GetProperty("messages")[0].GetProperty("content");
        Assert.Equal(JsonValueKind.Array, content.ValueKind);
        Assert.Equal("text", content[0].GetProperty("type").GetString());
        Assert.Equal("image_url", content[1].GetProperty("type").GetString());
        Assert.Equal("https://example.com/a.png", content[1].GetProperty("image_url").GetProperty("url").GetString());
    }

    [Fact]
    public async Task CreateAsync_SerializesToolChoiceFunction()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
            ToolChoice = ToolChoice.Function("get_weather"),
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        JsonElement choice = body.RootElement.GetProperty("tool_choice");
        Assert.Equal("function", choice.GetProperty("type").GetString());
        Assert.Equal("get_weather", choice.GetProperty("function").GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateAsync_SerializesToolChoiceMode()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/chat/completions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.Completion("ok")));

        using SkailarClient client = _fixture.CreateClient();
        await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
        {
            Model = "claude-sonnet-4-6",
            Messages = [ChatMessage.User("hi")],
            ToolChoice = ToolChoice.Required,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.Equal("required", body.RootElement.GetProperty("tool_choice").GetString());
    }
}
