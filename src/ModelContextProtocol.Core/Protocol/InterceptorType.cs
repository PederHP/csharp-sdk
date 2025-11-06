using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the type of an interceptor in the Model Context Protocol.
/// </summary>
/// <remarks>
/// Interceptor types define the primary purpose and execution model of an interceptor.
/// Each type has different execution characteristics and return value expectations.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<InterceptorType>))]
public enum InterceptorType
{
    /// <summary>
    /// Validation interceptors check inputs and outputs for correctness, security, or policy compliance.
    /// </summary>
    /// <remarks>
    /// Validation interceptors return structured validation results with severity levels (info/warn/error).
    /// Multiple validation interceptors can run in parallel. Validation results do not modify the payload.
    /// </remarks>
    Validation,

    /// <summary>
    /// Mutation interceptors transform message content sequentially.
    /// </summary>
    /// <remarks>
    /// Mutation interceptors modify the payload and return the transformed version.
    /// They execute sequentially in priority order, with each mutation feeding into the next.
    /// </remarks>
    Mutation,

    /// <summary>
    /// Observability interceptors perform fire-and-forget logging and metrics collection.
    /// </summary>
    /// <remarks>
    /// Observability interceptors are for logging, tracing, and metrics collection.
    /// They run asynchronously and do not block the request/response flow.
    /// Their results do not affect the payload or validation.
    /// </remarks>
    Observability
}
