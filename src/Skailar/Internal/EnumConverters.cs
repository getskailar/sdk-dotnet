using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar.Internal;

/// <summary>Serializes an enum using its camelCase member name (for single-word lowercase wire values).</summary>
internal sealed class CamelCaseEnumConverter<TEnum>()
    : JsonStringEnumConverter<TEnum>(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
    where TEnum : struct, Enum;

/// <summary>Serializes an enum using its snake_case member name (for multi-word wire values).</summary>
internal sealed class SnakeCaseEnumConverter<TEnum>()
    : JsonStringEnumConverter<TEnum>(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false)
    where TEnum : struct, Enum;
