using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the result of executing an interceptor chain via <see cref="RequestMethods.InterceptorExecuteChain"/>.
/// </summary>
/// <remarks>
/// <para>
/// The result aggregates outputs from all interceptors in the chain:
/// </para>
/// <list type="bullet">
/// <item><description>Mutation chains return the final transformed payload in <see cref="ModifiedPayload"/>.</description></item>
/// <item><description>Validation chains return all validation findings in <see cref="AllValidationResults"/>.</description></item>
/// <item><description>Mixed chains may return both transformed payloads and validation results.</description></item>
/// </list>
/// <para>
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </para>
/// </remarks>
public sealed class ExecuteChainResult
{
    /// <summary>
    /// Gets or sets the final modified payload after all mutation interceptors have been applied.
    /// </summary>
    /// <remarks>
    /// For mutation chains, this contains the result of sequentially applying all mutation interceptors.
    /// Each mutation interceptor transforms the payload from the previous step.
    /// </remarks>
    [JsonPropertyName("modifiedPayload")]
    public JsonElement? ModifiedPayload { get; set; }

    /// <summary>
    /// Gets or sets all validation results from all validation interceptors in the chain.
    /// </summary>
    /// <remarks>
    /// This aggregates validation findings from all validation interceptors.
    /// The array may contain results from multiple validators with different severity levels.
    /// </remarks>
    [JsonPropertyName("allValidationResults")]
    public ValidationResult[]? AllValidationResults { get; set; }

    /// <summary>
    /// Gets or sets aggregated metadata from all interceptors in the chain.
    /// </summary>
    /// <remarks>
    /// This can contain metrics, logs, or other observability data collected during chain execution.
    /// Keys may be namespaced by interceptor ID to avoid conflicts.
    /// </remarks>
    [JsonPropertyName("metadata")]
    public IDictionary<string, JsonElement>? Metadata { get; set; }
}
