using Skailar.Internal;

namespace Skailar;

/// <summary>The image generation endpoints.</summary>
public sealed class ImagesResource
{
    private readonly RequestExecutor _executor;

    internal ImagesResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Generates one or more images from a prompt.</summary>
    /// <param name="request">The generation request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generated images.</returns>
    public Task<ImageGenerationResponse> GenerateAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendJsonAsync(
            HttpMethod.Post,
            "v1/images/generations",
            request,
            SkailarJson.Default.ImageGenerationRequest,
            SkailarJson.Default.ImageGenerationResponse,
            Idempotency.SideEffect,
            perCallHeaders: null,
            cancellationToken);
}
