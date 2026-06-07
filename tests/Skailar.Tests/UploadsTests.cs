using System.Text.Json;
using Skailar;
using Skailar.Tests.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Skailar.Tests;

public sealed class UploadsTests(MockServerFixture fixture) : IClassFixture<MockServerFixture>
{
    private readonly MockServerFixture _fixture = fixture;

    [Fact]
    public async Task Images_CreateAsync_ReturnsUrl()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/uploads/images").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """{ "url": "/assets/img_1.png", "content_type": "image/png" }"""));

        using SkailarClient client = _fixture.CreateClient();
        UploadResponse response = await client.Uploads.Images.CreateAsync(new UploadImageRequest
        {
            Base64 = "aGVsbG8=",
            ContentType = ImageContentType.Png,
        });

        Assert.Equal("/assets/img_1.png", response.Url);
        Assert.Equal("image/png", response.ContentType);
    }

    [Fact]
    public async Task Images_SerializesContentTypeWithSlash()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/uploads/images").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """{ "url": "/x", "content_type": "image/webp" }"""));

        using SkailarClient client = _fixture.CreateClient();
        await client.Uploads.Images.CreateAsync(new UploadImageRequest
        {
            Base64 = "aGVsbG8=",
            ContentType = ImageContentType.Webp,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.SingleRequestBody());
        Assert.Equal("image/webp", body.RootElement.GetProperty("content_type").GetString());
        Assert.Equal("aGVsbG8=", body.RootElement.GetProperty("base64").GetString());
    }

    [Fact]
    public async Task Files_CreateAsync_ReturnsUrl()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/uploads/files").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """{ "url": "/assets/doc_1.pdf", "content_type": "application/pdf" }"""));

        using SkailarClient client = _fixture.CreateClient();
        UploadResponse response = await client.Uploads.Files.CreateAsync(new UploadFileRequest
        {
            Base64 = "JVBERi0=",
            ContentType = FileContentType.Pdf,
        });

        Assert.Equal("/assets/doc_1.pdf", response.Url);
    }

    [Fact]
    public async Task Files_SerializesTextContentType()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create().WithPath("/v1/uploads/files").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(
                """{ "url": "/x", "content_type": "text/plain" }"""));

        using SkailarClient client = _fixture.CreateClient();
        await client.Uploads.Files.CreateAsync(new UploadFileRequest
        {
            Base64 = "aGk=",
            ContentType = FileContentType.Text,
        });

        using JsonDocument body = JsonDocument.Parse(_fixture.SingleRequestBody());
        Assert.Equal("text/plain", body.RootElement.GetProperty("content_type").GetString());
    }
}
