using System.Text.Json.Serialization;

namespace Skailar;

/// <summary>The result of verifying an API key.</summary>
public sealed record PingKeyResponse
{
    /// <summary>The key status. <c>ok</c> when the key is valid.</summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>The identifier of the user that owns the key.</summary>
    [JsonPropertyName("user_id")]
    public required string UserId { get; init; }
}
