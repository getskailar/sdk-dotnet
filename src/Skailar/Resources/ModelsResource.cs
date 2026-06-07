using Skailar.Internal;

namespace Skailar;

/// <summary>The model-catalog endpoints.</summary>
public sealed class ModelsResource
{
    private readonly RequestExecutor _executor;

    internal ModelsResource(RequestExecutor executor) => _executor = executor;

    /// <summary>Lists every model the gateway can route to.</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The model catalog.</returns>
    public Task<ModelList> ListAsync(CancellationToken cancellationToken = default) =>
        _executor.GetJsonAsync("v1/models", SkailarJson.Default.ModelList, Idempotency.Idempotent, cancellationToken);

    /// <summary>Retrieves the full detail card for a single model.</summary>
    /// <param name="id">The model identifier. May contain slashes, for example <c>google/gemini-2.5-pro</c>.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The model detail.</returns>
    /// <exception cref="SkailarNotFoundException">No model with the given identifier exists.</exception>
    public Task<Model> RetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        string encoded = string.Join('/', id.Split('/').Select(Uri.EscapeDataString));
        return _executor.GetJsonAsync($"v1/models/{encoded}", SkailarJson.Default.Model, Idempotency.Idempotent, cancellationToken);
    }
}
