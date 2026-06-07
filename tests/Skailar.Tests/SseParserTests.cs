using Skailar.Internal;

namespace Skailar.Tests;

public sealed class SseParserTests
{
    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void Push_ParsesDataLines_AcrossAllTerminators(string nl)
    {
        var parser = new SseParser();
        string input = $"data: one{nl}data: two{nl}";

        List<string> events = parser.Push(input).ToList();

        Assert.Equal(["one", "two"], events);
    }

    [Fact]
    public void Push_HandlesCrLfSplitAcrossPushes()
    {
        var parser = new SseParser();

        List<string> first = parser.Push("data: hello\r").ToList();
        List<string> second = parser.Push("\ndata: world\n").ToList();

        Assert.Equal(["hello"], first);
        Assert.Equal(["world"], second);
    }

    [Fact]
    public void Push_IgnoresCommentsAndBlankLines()
    {
        var parser = new SseParser();
        string input = ": this is a comment\n\ndata: payload\n\n";

        List<string> events = parser.Push(input).ToList();

        Assert.Equal(["payload"], events);
    }

    [Fact]
    public void Push_StripsOnlyFirstSpaceAfterColon()
    {
        var parser = new SseParser();

        List<string> events = parser.Push("data:  two-spaces\n").ToList();

        Assert.Equal([" two-spaces"], events);
    }

    [Fact]
    public void Push_DataWithoutSpace_IsParsed()
    {
        var parser = new SseParser();

        List<string> events = parser.Push("data:nospace\n").ToList();

        Assert.Equal(["nospace"], events);
    }

    [Fact]
    public void Push_BuffersIncompleteLineUntilTerminator()
    {
        var parser = new SseParser();

        List<string> partial = parser.Push("data: incomp").ToList();
        List<string> completed = parser.Push("lete\n").ToList();

        Assert.Empty(partial);
        Assert.Equal(["incomplete"], completed);
    }

    [Fact]
    public void Push_EmitsDoneSentinelAsPayload()
    {
        var parser = new SseParser();

        List<string> events = parser.Push("data: [DONE]\n").ToList();

        Assert.Equal(["[DONE]"], events);
    }
}
