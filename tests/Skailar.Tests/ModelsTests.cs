using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class ModelsTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task ListAsync_ReturnsModels()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.ModelList()));

        using SkailarClient client = _fixture.CreateClient();
        ModelList list = await client.Models.ListAsync();

        Assert.Equal("list", list.Object);
        Assert.Equal("claude-sonnet-4-6", list.Data[0].Id);
        Assert.True(list.Data[0].Capabilities.Vision);
        Assert.Equal("USD", list.Data[0].Pricing.Currency);
    }

    [Fact]
    public async Task RetrieveAsync_ReturnsModelDetail()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models/gpt-5").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.ModelDetail("gpt-5")));

        using SkailarClient client = _fixture.CreateClient();
        Model model = await client.Models.RetrieveAsync("gpt-5");

        Assert.Equal("gpt-5", model.Id);
        Assert.Equal(["text", "image"], model.Modalities!.Input);
        Assert.Contains("gemini-pro", model.Aliases!);
    }

    [Fact]
    public async Task RetrieveAsync_EncodesSlashesInId()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/models/google/gemini-2.5-pro").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(Samples.ModelDetail("google/gemini-2.5-pro")));

        using SkailarClient client = _fixture.CreateClient();
        Model model = await client.Models.RetrieveAsync("google/gemini-2.5-pro");

        Assert.Equal("google/gemini-2.5-pro", model.Id);
    }

    [Fact]
    public async Task RetrieveAsync_EmptyId_Throws()
    {
        using SkailarClient client = _fixture.CreateClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.Models.RetrieveAsync(""));
    }
}
