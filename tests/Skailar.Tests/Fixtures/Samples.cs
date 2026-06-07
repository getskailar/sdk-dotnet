namespace Skailar.Tests.Fixtures;

/// <summary>Canned JSON response bodies mirroring the gateway's wire format.</summary>
internal static class Samples
{
    public static string Completion(string content) =>
        $$"""
        {
          "id": "chatcmpl-test",
          "object": "chat.completion",
          "created": 1700000000,
          "model": "claude-sonnet-4-6",
          "choices": [
            {
              "index": 0,
              "message": { "role": "assistant", "content": {{Json(content)}} },
              "finish_reason": "stop"
            }
          ],
          "usage": { "prompt_tokens": 5, "completion_tokens": 7, "total_tokens": 12 }
        }
        """;

    public static string Error(string type, string message) =>
        $$"""
        { "error": { "type": {{Json(type)}}, "message": {{Json(message)}} } }
        """;

    public static string ModelList() =>
        """
        {
          "object": "list",
          "data": [
            {
              "id": "claude-sonnet-4-6",
              "object": "model",
              "created": 1700000000,
              "owned_by": "anthropic",
              "display_name": "Claude Sonnet 4.6",
              "context_window": 200000,
              "max_output_tokens": 8192,
              "capabilities": { "streaming": true, "tool_calls": true, "vision": true, "json_mode": true, "reasoning": true },
              "pricing": { "input_per_mtok": 3.0, "output_per_mtok": 15.0, "currency": "USD" },
              "status": "active"
            }
          ]
        }
        """;

    public static string ModelDetail(string id) =>
        $$"""
        {
          "id": {{Json(id)}},
          "object": "model",
          "created": 1700000000,
          "owned_by": "google",
          "display_name": "Gemini 2.5 Pro",
          "context_window": 1000000,
          "max_output_tokens": 65536,
          "capabilities": { "streaming": true, "tool_calls": true, "vision": true, "json_mode": true, "reasoning": null },
          "pricing": { "input_per_mtok": 1.25, "output_per_mtok": 5.0, "currency": "USD" },
          "status": "active",
          "description": "A capable multimodal model.",
          "modalities": { "input": ["text", "image"], "output": ["text"] },
          "aliases": ["gemini-pro"]
        }
        """;

    public static string Streaming(params string[] tokens)
    {
        var lines = new List<string>();
        foreach (string token in tokens)
        {
            string delta = "{\"content\":" + Json(token) + "}";
            string chunk = "{\"id\":\"chatcmpl-test\",\"object\":\"chat.completion.chunk\",\"created\":1700000000," +
                "\"model\":\"claude-sonnet-4-6\",\"choices\":[{\"index\":0,\"delta\":" + delta + "}]}";
            lines.Add("data: " + chunk);
            lines.Add(string.Empty);
        }

        lines.Add("data: [DONE]");
        lines.Add(string.Empty);
        return string.Join('\n', lines);
    }

    private static string Json(string value) => System.Text.Json.JsonSerializer.Serialize(value);
}
