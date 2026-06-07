using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>The reasoning budget for reasoning-capable models.</summary>
[JsonConverter(typeof(CamelCaseEnumConverter<ReasoningEffort>))]
public enum ReasoningEffort
{
    /// <summary>Minimal reasoning before answering.</summary>
    Low,

    /// <summary>A balanced reasoning budget.</summary>
    Medium,

    /// <summary>Extended reasoning for harder problems.</summary>
    High,
}
