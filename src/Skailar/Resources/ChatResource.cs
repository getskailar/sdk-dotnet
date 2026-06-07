using Skailar.Internal;

namespace Skailar;

/// <summary>The chat endpoints.</summary>
public sealed class ChatResource
{
    /// <summary>The chat completions endpoint.</summary>
    public ChatCompletionsResource Completions { get; }

    internal ChatResource(RequestExecutor executor) => Completions = new ChatCompletionsResource(executor);
}
