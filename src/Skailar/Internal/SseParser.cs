using System.Text;

namespace Skailar.Internal;

/// <summary>
/// An incremental Server-Sent Events parser. Feed it decoded text via <see cref="Push"/>;
/// it emits the payload of each <c>data:</c> line. Accepts <c>\n</c>, <c>\r\n</c>, and
/// <c>\r</c> line terminators, deferring a lone trailing <c>\r</c> until the next push so a
/// <c>\r\n</c> split across reads is not mistaken for two line breaks.
/// </summary>
internal sealed class SseParser
{
    private const string DataPrefix = "data:";

    private readonly StringBuilder _line = new();
    private bool _pendingCr;

    /// <summary>Appends a chunk of decoded text and yields the payload of each complete <c>data:</c> line.</summary>
    /// <param name="text">The newly read text.</param>
    public IEnumerable<string> Push(string text)
    {
        var events = new List<string>();
        foreach (char c in text)
        {
            switch (c)
            {
                case '\r':
                    FlushLine(events);
                    _pendingCr = true;
                    break;
                case '\n':
                    if (_pendingCr)
                    {
                        _pendingCr = false;
                    }
                    else
                    {
                        FlushLine(events);
                    }

                    break;
                default:
                    _pendingCr = false;
                    _line.Append(c);
                    break;
            }
        }

        return events;
    }

    private void FlushLine(List<string> events)
    {
        string line = _line.ToString();
        _line.Clear();

        if (line.Length == 0 || line[0] == ':')
        {
            return;
        }

        if (!line.StartsWith(DataPrefix, StringComparison.Ordinal))
        {
            return;
        }

        string payload = line[DataPrefix.Length..];
        if (payload.StartsWith(' '))
        {
            payload = payload[1..];
        }

        events.Add(payload);
    }
}
