using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skailar.Internal;

/// <summary>Source-generated <see cref="JsonSerializerOptions"/> metadata for every wire type, enabling trim- and AOT-safe serialization.</summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ChatCompletionRequest))]
[JsonSerializable(typeof(ChatCompletionResponse))]
[JsonSerializable(typeof(ChatCompletionChunk))]
[JsonSerializable(typeof(ChatMessage))]
[JsonSerializable(typeof(MessageContent))]
[JsonSerializable(typeof(ContentPart))]
[JsonSerializable(typeof(TextPart))]
[JsonSerializable(typeof(ImagePart))]
[JsonSerializable(typeof(IReadOnlyList<ContentPart>))]
[JsonSerializable(typeof(Tool))]
[JsonSerializable(typeof(ToolCall))]
[JsonSerializable(typeof(ToolChoice))]
[JsonSerializable(typeof(ResponseFormat))]
[JsonSerializable(typeof(Usage))]
[JsonSerializable(typeof(ModelList))]
[JsonSerializable(typeof(ModelSummary))]
[JsonSerializable(typeof(Model))]
[JsonSerializable(typeof(ImageGenerationRequest))]
[JsonSerializable(typeof(ImageGenerationResponse))]
[JsonSerializable(typeof(TranscriptionRequest))]
[JsonSerializable(typeof(TranscriptionResponse))]
[JsonSerializable(typeof(SpeechRequest))]
[JsonSerializable(typeof(UploadImageRequest))]
[JsonSerializable(typeof(UploadFileRequest))]
[JsonSerializable(typeof(UploadResponse))]
[JsonSerializable(typeof(PingKeyResponse))]
[JsonSerializable(typeof(JsonElement))]
internal sealed partial class SkailarJsonContext : JsonSerializerContext;
