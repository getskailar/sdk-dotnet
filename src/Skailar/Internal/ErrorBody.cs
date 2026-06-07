using System.Text.Json;

namespace Skailar.Internal;

/// <summary>Parses the structured error body returned by the gateway.</summary>
internal static class ErrorBody
{
    /// <summary>
    /// Extracts a machine-readable code and human-readable message from an error payload,
    /// tolerating both the flat <c>{ "error": "code", "message": "..." }</c> form and the
    /// nested <c>{ "error": { "type" | "code": "...", "message": "..." } }</c> form.
    /// </summary>
    /// <param name="raw">The raw response body.</param>
    /// <returns>The parsed code and message, either of which may be <see langword="null"/>.</returns>
    public static (string? Code, string? Message) Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(raw);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, null);
            }

            string? topMessage = GetString(root, "message");

            if (!root.TryGetProperty("error", out JsonElement error))
            {
                return (null, topMessage);
            }

            if (error.ValueKind == JsonValueKind.String)
            {
                return (error.GetString(), topMessage);
            }

            if (error.ValueKind == JsonValueKind.Object)
            {
                string? code = GetString(error, "type") ?? GetString(error, "code");
                string? message = GetString(error, "message") ?? topMessage;
                return (code, message);
            }

            return (null, topMessage);
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }

    private static string? GetString(JsonElement element, string property) =>
        element.TryGetProperty(property, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
}
