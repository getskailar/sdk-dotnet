using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>The author role of a chat message.</summary>
[JsonConverter(typeof(CamelCaseEnumConverter<ChatRole>))]
public enum ChatRole
{
    /// <summary>A system instruction that steers the assistant.</summary>
    System,

    /// <summary>A message from the end user.</summary>
    User,

    /// <summary>A message produced by the assistant.</summary>
    Assistant,

    /// <summary>The result of a tool call, supplied back to the model.</summary>
    Tool,
}
