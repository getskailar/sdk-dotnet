using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>The reason the model stopped generating tokens.</summary>
[JsonConverter(typeof(SnakeCaseEnumConverter<FinishReason>))]
public enum FinishReason
{
    /// <summary>The model reached a natural stopping point or a stop sequence.</summary>
    Stop,

    /// <summary>Generation hit the maximum token budget.</summary>
    Length,

    /// <summary>The model emitted one or more tool calls.</summary>
    ToolCalls,

    /// <summary>Content was withheld by the provider's content filter.</summary>
    ContentFilter,
}
