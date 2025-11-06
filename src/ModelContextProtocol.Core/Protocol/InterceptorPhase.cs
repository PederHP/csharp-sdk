using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the phase at which an interceptor can execute.
/// </summary>
/// <remarks>
/// Interceptors can operate on requests before they are processed (Request phase)
/// or on responses before they are sent back (Response phase).
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<InterceptorPhase>))]
public enum InterceptorPhase
{
    /// <summary>
    /// The interceptor executes on incoming requests before they are processed.
    /// </summary>
    /// <remarks>
    /// Request-phase interceptors can validate or transform incoming messages
    /// before the server processes them.
    /// </remarks>
    Request,

    /// <summary>
    /// The interceptor executes on outgoing responses before they are sent.
    /// </summary>
    /// <remarks>
    /// Response-phase interceptors can validate or transform outgoing messages
    /// before they are sent to the client.
    /// </remarks>
    Response
}
