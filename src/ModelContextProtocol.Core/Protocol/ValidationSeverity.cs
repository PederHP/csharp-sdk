using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the severity level of a validation result.
/// </summary>
/// <remarks>
/// Validation severity indicates the importance of a validation finding.
/// Higher severity levels may trigger different handling behaviors.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<ValidationSeverity>))]
public enum ValidationSeverity
{
    /// <summary>
    /// Informational message that does not indicate a problem.
    /// </summary>
    /// <remarks>
    /// Info-level validations provide additional context or suggestions
    /// but do not indicate any policy violation or error.
    /// </remarks>
    Info,

    /// <summary>
    /// Warning about a potential issue that should be reviewed.
    /// </summary>
    /// <remarks>
    /// Warnings indicate potential problems or policy concerns that may require attention
    /// but do not prevent the operation from proceeding.
    /// </remarks>
    Warning,

    /// <summary>
    /// Error indicating a validation failure that should prevent the operation.
    /// </summary>
    /// <remarks>
    /// Errors indicate validation failures that violate policies or requirements.
    /// Operations with error-level validation results should typically be rejected.
    /// </remarks>
    Error
}
