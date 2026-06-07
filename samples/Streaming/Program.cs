using Skailar;

using var client = new SkailarClient();

var request = new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages = [ChatMessage.User("Write a haiku about streaming data.")],
};

await foreach (ChatCompletionChunk chunk in client.Chat.Completions.CreateStreamAsync(request))
{
    if (chunk.Choices[0].Delta.Content is { } text)
    {
        Console.Write(text);
    }
}

Console.WriteLine();
