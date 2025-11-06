using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the parameters used with a <see cref="RequestMethods.InterceptorExecuteChain"/> request
/// to execute multiple interceptors in sequence.
/// </summary>
/// <remarks>
/// <para>
/// Chain execution follows specific rules:
/// </para>
/// <list type="bullet">
/// <item><description>Mutation interceptors execute sequentially in priority order (lower priority first).</description></item>
/// <item><description>Validation interceptors can run in parallel.</description></item>
/// <item><description>Observability interceptors run asynchronously (fire-and-forget).</description></item>
/// </list>
/// <para>
/// The server responds with an <see cref="ExecuteChainResult"/> containing the aggregated results.
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </para>
/// </remarks>
public sealed class ExecuteChainRequestParams
{
    /// <summary>
    /// Gets or sets the ordered list of interceptor IDs to execute.
    /// </summary>
    /// <remarks>
    /// Interceptors will be executed according to their priority and type.
    /// The order in this array provides a hint but may be reordered based on priority and execution model.
    /// </remarks>
    [JsonPropertyName("interceptorIds")]
    [JsonRequired]
    public required string[] InterceptorIds { get; set; }

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
    /// Gets or sets the phase at which the interceptors are being invoked.
    /// </summary>
    [JsonPropertyName("phase")]
    [JsonRequired]
    public required InterceptorPhase Phase { get; set; }

    /// <summary>
    /// Gets or sets the initial payload to be processed by the interceptor chain.
    /// </summary>
    /// <remarks>
    /// For mutation chains, this payload will be sequentially transformed by each mutation interceptor.
    /// For validation chains, this payload will be validated by all validation interceptors.
    /// </remarks>
    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }

    /// <summary>
    /// Gets or sets an optional progress token for receiving progress notifications during chain execution.
    /// </summary>
    /// <remarks>
    /// When provided, the server may send <see cref="NotificationMethods.ProgressNotification"/> notifications
    /// with this token to report progress during chain execution.
    /// </remarks>
    [JsonPropertyName("_meta")]
    public ProgressMeta? Meta { get; set; }
}
