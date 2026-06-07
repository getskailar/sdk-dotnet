using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>The MIME type of an audio clip submitted for transcription.</summary>
[JsonConverter(typeof(SkailarMimeConverter))]
public enum SkailarMime
{
    /// <summary><c>audio/wav</c>.</summary>
    Wav,

    /// <summary><c>audio/webm</c>.</summary>
    Webm,

    /// <summary><c>audio/mp4</c>.</summary>
    Mp4,

    /// <summary><c>audio/m4a</c>.</summary>
    M4a,

    /// <summary><c>audio/mpeg</c>.</summary>
    Mpeg,

    /// <summary><c>audio/mp3</c>.</summary>
    Mp3,
}

/// <summary>Maps <see cref="SkailarMime"/> values to and from their wire strings.</summary>
internal sealed class SkailarMimeConverter : JsonConverter<SkailarMime>
{
    public override SkailarMime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "audio/wav" => SkailarMime.Wav,
            "audio/webm" => SkailarMime.Webm,
            "audio/mp4" => SkailarMime.Mp4,
            "audio/m4a" => SkailarMime.M4a,
            "audio/mpeg" => SkailarMime.Mpeg,
            "audio/mp3" => SkailarMime.Mp3,
            var other => throw new JsonException($"Unknown audio MIME type '{other}'."),
        };

    public override void Write(Utf8JsonWriter writer, SkailarMime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            SkailarMime.Wav => "audio/wav",
            SkailarMime.Webm => "audio/webm",
            SkailarMime.Mp4 => "audio/mp4",
            SkailarMime.M4a => "audio/m4a",
            SkailarMime.Mpeg => "audio/mpeg",
            SkailarMime.Mp3 => "audio/mp3",
            _ => throw new JsonException($"Unknown audio MIME type '{value}'."),
        });
}
