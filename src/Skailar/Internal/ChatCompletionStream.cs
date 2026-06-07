using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Skailar.Internal;

/// <summary>
/// Streams <see cref="ChatCompletionChunk"/> values from an SSE response, disposing the
/// underlying HTTP response when enumeration completes or the consumer stops early.
/// </summary>
internal static class ChatCompletionStream
{
    private const string DoneSentinel = "[DONE]";

    /// <summary>Reads, parses, and yields chunks from the given SSE response.</summary>
    /// <param name="response">The live SSE response; owned by this method and disposed on completion.</param>
    /// <param name="cancellationToken">A token to stop enumeration.</param>
    public static async IAsyncEnumerable<ChatCompletionChunk> ReadAsync(
        HttpResponseMessage response,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parser = new SseParser();
        var decoder = Encoding.UTF8.GetDecoder();
        var buffer = new byte[8192];

        try
        {
            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                while (true)
                {
                    int read = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                    if (read == 0)
                    {
                        break;
                    }

                    foreach (ChatCompletionChunk chunk in DecodeChunks(decoder, buffer, read, parser))
                    {
                        yield return chunk;
                    }
                }
            }
        }
        finally
        {
            response.Dispose();
        }
    }

    private static IEnumerable<ChatCompletionChunk> DecodeChunks(Decoder decoder, byte[] buffer, int read, SseParser parser)
    {
        int charCount = decoder.GetCharCount(buffer, 0, read);
        char[] chars = new char[charCount];
        int decoded = decoder.GetChars(buffer, 0, read, chars, 0);
        string text = new(chars, 0, decoded);

        var chunks = new List<ChatCompletionChunk>();
        foreach (string payload in parser.Push(text))
        {
            if (payload == DoneSentinel)
            {
                break;
            }

            chunks.Add(ParsePayload(payload));
        }

        return chunks;
    }

    private static ChatCompletionChunk ParsePayload(string payload)
    {
        ThrowIfInBandError(payload);

        try
        {
            ChatCompletionChunk? chunk = JsonSerializer.Deserialize(payload, SkailarJson.Default.ChatCompletionChunk);
            return chunk ?? throw new SkailarException("Received an empty streaming chunk.", SkailarErrorKind.Decode);
        }
        catch (JsonException ex)
        {
            throw new SkailarConnectionException($"Malformed streaming event (not valid JSON): {Truncate(payload)}", ex);
        }
    }

    private static void ThrowIfInBandError(string payload)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("error", out _))
            {
                (string? code, string? message) = ErrorBody.Parse(payload);
                throw new SkailarUpstreamException(message ?? "Streaming error.", status: 500, code: code, raw: payload);
            }
        }
        catch (JsonException ex)
        {
            throw new SkailarConnectionException($"Malformed streaming event (not valid JSON): {Truncate(payload)}", ex);
        }
    }

    private static string Truncate(string payload) =>
        payload.Length <= 200 ? payload : payload[..200];
}
