using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents a validation result from an interceptor.
/// </summary>
/// <remarks>
/// Validation results are returned by validation-type interceptors to indicate
/// issues, warnings, or informational messages about the payload being validated.
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </remarks>
public sealed class ValidationResult
{
    /// <summary>
    /// Gets or sets the severity level of this validation result.
    /// </summary>
    [JsonPropertyName("severity")]
    [JsonRequired]
    public required ValidationSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the human-readable message describing the validation result.
    /// </summary>
    [JsonPropertyName("message")]
    [JsonRequired]
    public required string Message { get; set; }

    /// <summary>
    /// Gets or sets an optional JSON path indicating the specific location in the payload
    /// where the validation issue was found.
    /// </summary>
    /// <remarks>
    /// The path follows JSON path notation (e.g., "$.tools[0].name" or "arguments.userId").
    /// This helps pinpoint the exact location of validation issues in complex payloads.
    /// </remarks>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}
