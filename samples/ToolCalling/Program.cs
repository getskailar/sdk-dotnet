using System.Text.Json;
using Skailar;

using var client = new SkailarClient();

JsonElement parameters = JsonDocument.Parse(
    """
    {
      "type": "object",
      "properties": {
        "city": { "type": "string", "description": "City name" }
      },
      "required": ["city"]
    }
    """).RootElement;

var request = new ChatCompletionRequest
{
    Model = SkailarModels.ClaudeSonnet4_6,
    Messages = [ChatMessage.User("What's the weather in Rome?")],
    Tools = [Tool.CreateFunction("get_weather", "Look up the current weather for a city.", parameters)],
    ToolChoice = ToolChoice.Auto,
};

ChatCompletionResponse response = await client.Chat.Completions.CreateAsync(request);

foreach (ToolCall call in response.Choices[0].Message.ToolCalls ?? [])
{
    Console.WriteLine($"Tool: {call.Function.Name}");
    Console.WriteLine($"Arguments: {call.Function.Arguments}");
}
