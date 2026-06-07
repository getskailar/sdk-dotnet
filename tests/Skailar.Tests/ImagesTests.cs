using System.Text.Json;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class ImagesTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task GenerateAsync_ReturnsImages()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/images/generations").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """
                { "created": 1700000000, "data": [ { "b64_json": "aGVsbG8=", "revised_prompt": "a cat" } ] }
                """));

        using SkailarClient client = _fixture.CreateClient();
        ImageGenerationResponse response = await client.Images.GenerateAsync(new ImageGenerationRequest
        {
            Model = SkailarModels.GptImage1,
            Prompt = "a cat",
            N = 1,
            Size = "1024x1024",
        });

        Assert.Equal("aGVsbG8=", response.Data[0].B64Json);
        Assert.Equal("a cat", response.Data[0].RevisedPrompt);
    }

    [Fact]
    public async Task GenerateAsync_SerializesRequestFields()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/images/generations").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """{ "created": 1, "data": [] }"""));

        using SkailarClient client = _fixture.CreateClient();
        await client.Images.GenerateAsync(new ImageGenerationRequest
        {
            Model = "gpt-image-1",
            Prompt = "sunset",
            Size = "1792x1024",
            Quality = "hd",
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.Equal("sunset", body.RootElement.GetProperty("prompt").GetString());
        Assert.Equal("1792x1024", body.RootElement.GetProperty("size").GetString());
        Assert.Equal("hd", body.RootElement.GetProperty("quality").GetString());
    }
}
