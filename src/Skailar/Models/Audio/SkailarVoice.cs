using System.Text.Json.Serialization;
using Skailar.Internal;

namespace Skailar;

/// <summary>A synthesized speech voice.</summary>
[JsonConverter(typeof(CamelCaseEnumConverter<SkailarVoice>))]
public enum SkailarVoice
{
    /// <summary>The <c>alloy</c> voice.</summary>
    Alloy,

    /// <summary>The <c>ash</c> voice.</summary>
    Ash,

    /// <summary>The <c>ballad</c> voice.</summary>
    Ballad,

    /// <summary>The <c>coral</c> voice.</summary>
    Coral,

    /// <summary>The <c>echo</c> voice.</summary>
    Echo,

    /// <summary>The <c>fable</c> voice.</summary>
    Fable,

    /// <summary>The <c>nova</c> voice.</summary>
    Nova,

    /// <summary>The <c>onyx</c> voice.</summary>
    Onyx,

    /// <summary>The <c>sage</c> voice.</summary>
    Sage,

    /// <summary>The <c>shimmer</c> voice.</summary>
    Shimmer,
}
