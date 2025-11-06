using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the result of invoking an interceptor via <see cref="RequestMethods.InterceptorInvoke"/>.
/// </summary>
/// <remarks>
/// <para>
/// The structure of the result depends on the interceptor type:
/// </para>
/// <list type="bullet">
/// <item><description>Mutation interceptors return a modified payload in <see cref="ModifiedPayload"/>.</description></item>
/// <item><description>Validation interceptors return validation findings in <see cref="ValidationResults"/>.</description></item>
/// <item><description>Observability interceptors may return metadata about what was logged/recorded in <see cref="Metadata"/>.</description></item>
/// </list>
/// <para>
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </para>
/// </remarks>
public sealed class InvokeInterceptorResult
{
    /// <summary>
    /// Gets or sets the modified payload for mutation-type interceptors.
    /// </summary>
    /// <remarks>
    /// This field is populated by mutation interceptors and contains the transformed message content.
    /// For validation and observability interceptors, this field is typically null.
    /// </remarks>
    [JsonPropertyName("modifiedPayload")]
    public JsonElement? ModifiedPayload { get; set; }

    /// <summary>
    /// Gets or sets validation results for validation-type interceptors.
    /// </summary>
    /// <remarks>
    /// This field is populated by validation interceptors and contains structured validation findings
    /// with severity levels. For mutation and observability interceptors, this field is typically null.
    /// </remarks>
    [JsonPropertyName("validationResults")]
    public ValidationResult[]? ValidationResults { get; set; }

    /// <summary>
    /// Gets or sets optional metadata from the interceptor execution.
    /// </summary>
    /// <remarks>
    /// This field can contain additional information about the interceptor execution,
    /// such as metrics collected, logs generated, or other observability data.
    /// </remarks>
    [JsonPropertyName("metadata")]
    public IDictionary<string, JsonElement>? Metadata { get; set; }
}
