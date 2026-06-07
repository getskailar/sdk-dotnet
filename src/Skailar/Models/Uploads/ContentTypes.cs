using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>The content type of an image uploaded to Skailar storage.</summary>
[JsonConverter(typeof(ImageContentTypeConverter))]
public enum ImageContentType
{
    /// <summary><c>image/png</c>.</summary>
    Png,

    /// <summary><c>image/jpeg</c>.</summary>
    Jpeg,

    /// <summary><c>image/gif</c>.</summary>
    Gif,

    /// <summary><c>image/webp</c>.</summary>
    Webp,
}

/// <summary>The content type of a document uploaded to Skailar storage.</summary>
[JsonConverter(typeof(FileContentTypeConverter))]
public enum FileContentType
{
    /// <summary><c>application/pdf</c>.</summary>
    Pdf,

    /// <summary><c>text/plain</c>.</summary>
    Text,
}

/// <summary>Maps <see cref="ImageContentType"/> values to and from their wire strings.</summary>
internal sealed class ImageContentTypeConverter : JsonConverter<ImageContentType>
{
    public override ImageContentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "image/png" => ImageContentType.Png,
            "image/jpeg" => ImageContentType.Jpeg,
            "image/gif" => ImageContentType.Gif,
            "image/webp" => ImageContentType.Webp,
            var other => throw new JsonException($"Unknown image content type '{other}'."),
        };

    public override void Write(Utf8JsonWriter writer, ImageContentType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ImageContentType.Png => "image/png",
            ImageContentType.Jpeg => "image/jpeg",
            ImageContentType.Gif => "image/gif",
            ImageContentType.Webp => "image/webp",
            _ => throw new JsonException($"Unknown image content type '{value}'."),
        });
}

/// <summary>Maps <see cref="FileContentType"/> values to and from their wire strings.</summary>
internal sealed class FileContentTypeConverter : JsonConverter<FileContentType>
{
    public override FileContentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "application/pdf" => FileContentType.Pdf,
            "text/plain" => FileContentType.Text,
            var other => throw new JsonException($"Unknown file content type '{other}'."),
        };

    public override void Write(Utf8JsonWriter writer, FileContentType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            FileContentType.Pdf => "application/pdf",
            FileContentType.Text => "text/plain",
            _ => throw new JsonException($"Unknown file content type '{value}'."),
        });
}
