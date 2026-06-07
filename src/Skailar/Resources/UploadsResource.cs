using Skailar.Internal;

namespace Skailar;

/// <summary>The storage upload endpoints.</summary>
public sealed class UploadsResource
{
    /// <summary>The image upload endpoint.</summary>
    public ImageUploadsResource Images { get; }

    /// <summary>The document upload endpoint.</summary>
    public FileUploadsResource Files { get; }

    internal UploadsResource(RequestExecutor executor)
    {
        Images = new ImageUploadsResource(executor);
        Files = new FileUploadsResource(executor);
    }
}
