using System.Text.Json;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class AudioTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task Transcriptions_CreateAsync_ReturnsText()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/audio/transcriptions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "text": "hello world" }"""));

        using SkailarClient client = _fixture.CreateClient();
        TranscriptionResponse response = await client.Audio.Transcriptions.CreateAsync(new TranscriptionRequest
        {
            Base64 = "QUJD",
            Mime = SkailarMime.Mp3,
        });

        Assert.Equal("hello world", response.Text);
    }

    [Fact]
    public async Task Transcriptions_SerializesMimeWithSlash()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/audio/transcriptions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{ "text": "x" }"""));

        using SkailarClient client = _fixture.CreateClient();
        await client.Audio.Transcriptions.CreateAsync(new TranscriptionRequest
        {
            Base64 = "QUJD",
            Mime = SkailarMime.Webm,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.Equal("audio/webm", body.RootElement.GetProperty("mime").GetString());
    }

    [Fact]
    public async Task Speech_CreateAsync_ReturnsAudioStream()
    {
        _fixture.Reset();
        byte[] mp3 = [0xFF, 0xFB, 0x90, 0x00, 0x01, 0x02, 0x03];
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/audio/speech").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "audio/mpeg").WithBody(mp3));

        using SkailarClient client = _fixture.CreateClient();
        await using Stream audio = await client.Audio.Speech.CreateAsync(new SpeechRequest
        {
            Input = "hello",
            Voice = SkailarVoice.Nova,
        });

        using var buffer = new MemoryStream();
        await audio.CopyToAsync(buffer);
        Assert.Equal(mp3, buffer.ToArray());
    }

    [Fact]
    public async Task Speech_SerializesVoice()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/audio/speech").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "audio/mpeg").WithBody([0x00]));

        using SkailarClient client = _fixture.CreateClient();
        await using Stream _ = await client.Audio.Speech.CreateAsync(new SpeechRequest
        {
            Input = "hi",
            Voice = SkailarVoice.Shimmer,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.Server.LogEntries.Single().RequestMessage.Body!);
        Assert.Equal("shimmer", body.RootElement.GetProperty("voice").GetString());
    }

    [Fact]
    public async Task Speech_ErrorResponse_ThrowsApiException()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/audio/speech").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(400).WithBody(Samples.Error("bad_request", "too long")));

        using SkailarClient client = _fixture.CreateClient();
        await Assert.ThrowsAsync<SkailarBadRequestException>(() => client.Audio.Speech.CreateAsync(new SpeechRequest
        {
            Input = "hi",
        }));
    }
}
