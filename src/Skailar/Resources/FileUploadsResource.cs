using Skailar.Internal;

namespace Skailar;

/// <summary>The document upload endpoint.</summary>
public sealed class FileUploadsResource
{
    private readonly RequestExecutor _executor;

    internal FileUploadsResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Uploads a base64-encoded document and returns its stored URL.</summary>
    /// <param name="request">The upload request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The stored-asset URL.</returns>
    public Task<UploadResponse> CreateAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendJsonAsync(
            HttpMethod.Post,
            "v1/uploads/files",
            request,
            SkailarJson.Default.UploadFileRequest,
            SkailarJson.Default.UploadResponse,
            Idempotency.SideEffect,
            perCallHeaders: null,
            cancellationToken);
}
