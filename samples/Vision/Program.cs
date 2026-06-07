using Skailar;

using var client = new SkailarClient();

ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages =
    [
        ChatMessage.User(
        [
            ContentPart.Text("What is in this image?"),
            ContentPart.Image("https://upload.wikimedia.org/wikipedia/commons/3/3a/Cat03.jpg", detail: "high"),
        ]),
    ],
});

Console.WriteLine(response.Choices[0].Message.Content?.GetText());
