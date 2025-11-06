using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the parameters used with a <see cref="RequestMethods.InterceptorInvoke"/> request
/// to invoke a single interceptor.
/// </summary>
/// <remarks>
/// The server responds with an <see cref="InvokeInterceptorResult"/> containing the interceptor's output.
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </remarks>
public sealed class InvokeInterceptorRequestParams
{
    /// <summary>
    /// Gets or sets the unique identifier of the interceptor to invoke.
    /// </summary>
    [JsonPropertyName("interceptorId")]
    [JsonRequired]
    public required string InterceptorId { get; set; }

    /// <summary>
    /// Gets or sets the event name being intercepted.
    /// </summary>
    /// <remarks>
    /// Event names correspond to MCP protocol methods (e.g., "tools/call", "resources/read", "prompts/get").
    /// </remarks>
    [JsonPropertyName("event")]
    [JsonRequired]
    public required string Event { get; set; }

    /// <summary>
    /// Gets or sets the phase at which the interceptor is being invoked.
    /// </summary>
    [JsonPropertyName("phase")]
    [JsonRequired]
    public required InterceptorPhase Phase { get; set; }

    /// <summary>
    /// Gets or sets the payload to be processed by the interceptor.
    /// </summary>
    /// <remarks>
    /// The payload is the message content being intercepted. For mutation interceptors,
    /// this will be transformed. For validation interceptors, this will be validated.
    /// For observability interceptors, this will be logged or recorded.
    /// </remarks>
    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }

    /// <summary>
    /// Gets or sets an optional progress token for receiving progress notifications during interceptor execution.
    /// </summary>
    /// <remarks>
    /// When provided, the server may send <see cref="NotificationMethods.ProgressNotification"/> notifications
    /// with this token to report progress during interceptor execution.
    /// </remarks>
    [JsonPropertyName("_meta")]
    public ProgressMeta? Meta { get; set; }
}
