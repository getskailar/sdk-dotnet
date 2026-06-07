namespace Skailar.Internal;

/// <summary>Whether a request may be safely retried after a server or transport failure.</summary>
internal enum Idempotency
{
    /// <summary>Safe to retry on 5xx and transient transport failures (for example, GET requests).</summary>
    Idempotent,

    /// <summary>Has side effects; never retried on 5xx or transport failures to avoid duplicate billing.</summary>
    SideEffect,
}
