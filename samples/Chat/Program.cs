using Skailar;

using var client = new SkailarClient();

ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages =
    [
        ChatMessage.System("You are a concise assistant."),
        ChatMessage.User("In one sentence, what is Skailar?"),
    ],
});

Console.WriteLine(response.Choices[0].Message.Content?.GetText());
Console.WriteLine($"\nTokens: {response.Usage?.TotalTokens}");
