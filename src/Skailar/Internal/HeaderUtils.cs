namespace Skailar.Internal;

/// <summary>Helpers for merging request headers while protecting the SDK-owned <c>Authorization</c> header.</summary>
internal static class HeaderUtils
{
    private const string Authorization = "authorization";

    /// <summary>
    /// Merges default and per-call headers into a case-insensitive map, with per-call values
    /// taking precedence. Any caller-supplied <c>Authorization</c> header is dropped so it can
    /// never shadow the SDK's bearer token.
    /// </summary>
    /// <param name="defaults">The client-wide default headers.</param>
    /// <param name="perCall">The per-request header overrides, if any.</param>
    public static Dictionary<string, string> Merge(
        IReadOnlyDictionary<string, string> defaults,
        IReadOnlyDictionary<string, string>? perCall)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in defaults)
        {
            if (!IsAuthorization(key))
            {
                merged[key] = value;
            }
        }

        if (perCall is not null)
        {
            foreach (var (key, value) in perCall)
            {
                if (!IsAuthorization(key))
                {
                    merged[key] = value;
                }
            }
        }

        return merged;
    }

    private static bool IsAuthorization(string name) =>
        string.Equals(name, Authorization, StringComparison.OrdinalIgnoreCase);
}
