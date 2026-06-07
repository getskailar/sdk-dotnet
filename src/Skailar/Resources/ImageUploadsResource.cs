using Skailar.Internal;

namespace Skailar;

/// <summary>The image upload endpoint.</summary>
public sealed class ImageUploadsResource
{
    private readonly RequestExecutor _executor;

    internal ImageUploadsResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Uploads a base64-encoded image and returns its stored URL.</summary>
    /// <param name="request">The upload request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The stored-asset URL.</returns>
    public Task<UploadResponse> CreateAsync(
        UploadImageRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendJsonAsync(
            HttpMethod.Post,
            "v1/uploads/images",
            request,
            SkailarJson.Default.UploadImageRequest,
            SkailarJson.Default.UploadResponse,
            Idempotency.SideEffect,
            perCallHeaders: null,
            cancellationToken);
}
