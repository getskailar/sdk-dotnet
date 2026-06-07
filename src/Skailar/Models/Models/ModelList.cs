using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>A list of models returned by <c>GET /v1/models</c>.</summary>
public sealed record ModelList
{
    /// <summary>The object type. Always <c>list</c>.</summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "list";

    /// <summary>The model entries.</summary>
    [JsonPropertyName("data")]
    public required IReadOnlyList<ModelSummary> Data { get; init; }
}
