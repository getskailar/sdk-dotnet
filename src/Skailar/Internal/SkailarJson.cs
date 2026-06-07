using System.Text.Json;

namespace Skailar.Internal;

/// <summary>The shared, source-generated serialization context used for all wire payloads.</summary>
internal static class SkailarJson
{
    /// <summary>The default context: omits null properties on write and matches property names case-insensitively.</summary>
    public static SkailarJsonContext Default => SkailarJsonContext.Default;

    /// <summary>Serializes a value to a UTF-8 JSON byte array using its generated type metadata.</summary>
    public static byte[] SerializeToUtf8Bytes<T>(T value, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo) =>
        JsonSerializer.SerializeToUtf8Bytes(value, typeInfo);
}
