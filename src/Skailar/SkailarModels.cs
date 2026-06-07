namespace Skailar;

/// <summary>
/// Well-known model identifiers accepted by the Skailar gateway. Call
/// <c>client.Models.ListAsync()</c> for the authoritative live catalog.
/// </summary>
public static class SkailarModels
{
    /// <summary>Claude Opus 4.8 (Anthropic).</summary>
    public const string ClaudeOpus4_8 = "claude-opus-4-8";

    /// <summary>Claude Opus 4.7 (Anthropic).</summary>
    public const string ClaudeOpus4_7 = "claude-opus-4-7";

    /// <summary>Claude Opus 4.6 (Anthropic).</summary>
    public const string ClaudeOpus4_6 = "claude-opus-4-6";

    /// <summary>Claude Sonnet 4.6 (Anthropic).</summary>
    public const string ClaudeSonnet4_6 = "claude-sonnet-4-6";

    /// <summary>Claude Sonnet 4.5 (Anthropic).</summary>
    public const string ClaudeSonnet4_5 = "claude-sonnet-4-5";

    /// <summary>Claude Haiku 4.5 (Anthropic).</summary>
    public const string ClaudeHaiku4_5 = "claude-haiku-4-5";

    /// <summary>GPT-5.5 (OpenAI).</summary>
    public const string Gpt5_5 = "gpt-5.5";

    /// <summary>GPT-5.4 (OpenAI).</summary>
    public const string Gpt5_4 = "gpt-5.4";

    /// <summary>GPT-5.4 mini (OpenAI).</summary>
    public const string Gpt5_4Mini = "gpt-5.4-mini";

    /// <summary>GPT-5.4 nano (OpenAI).</summary>
    public const string Gpt5_4Nano = "gpt-5.4-nano";

    /// <summary>GPT-5.1 (OpenAI).</summary>
    public const string Gpt5_1 = "gpt-5.1";

    /// <summary>GPT-5 (OpenAI).</summary>
    public const string Gpt5 = "gpt-5";

    /// <summary>GPT-5 mini (OpenAI).</summary>
    public const string Gpt5Mini = "gpt-5-mini";

    /// <summary>o3 reasoning model (OpenAI).</summary>
    public const string O3 = "o3";

    /// <summary>o4-mini reasoning model (OpenAI).</summary>
    public const string O4Mini = "o4-mini";

    /// <summary>Gemini 3.5 Flash (Google).</summary>
    public const string Gemini3_5Flash = "gemini-3.5-flash";

    /// <summary>Gemini 3.1 Pro preview (Google).</summary>
    public const string Gemini3_1ProPreview = "gemini-3.1-pro-preview";

    /// <summary>Gemini 3 Flash preview (Google).</summary>
    public const string Gemini3FlashPreview = "gemini-3-flash-preview";

    /// <summary>Gemini 2.5 Pro (Google).</summary>
    public const string Gemini2_5Pro = "gemini-2.5-pro";

    /// <summary>Gemini 2.5 Flash (Google).</summary>
    public const string Gemini2_5Flash = "gemini-2.5-flash";

    /// <summary>Gemini 2.5 Flash Lite (Google).</summary>
    public const string Gemini2_5FlashLite = "gemini-2.5-flash-lite";

    /// <summary>DeepSeek V4 Pro (DeepSeek).</summary>
    public const string DeepSeekV4Pro = "deepseek-v4-pro";

    /// <summary>DeepSeek V4 Flash (DeepSeek).</summary>
    public const string DeepSeekV4Flash = "deepseek-v4-flash";

    /// <summary>Grok 4.3 (xAI).</summary>
    public const string Grok4_3 = "grok-4.3";

    /// <summary>Grok 4.20 reasoning (xAI).</summary>
    public const string Grok4_20Reasoning = "grok-4.20-reasoning";

    /// <summary>Grok 4.20 non-reasoning (xAI).</summary>
    public const string Grok4_20NonReasoning = "grok-4.20-non-reasoning";

    /// <summary>Grok Build 0.1 (xAI).</summary>
    public const string GrokBuild0_1 = "grok-build-0.1";

    /// <summary>GPT Image 1 (OpenAI), for image generation.</summary>
    public const string GptImage1 = "gpt-image-1";
}
