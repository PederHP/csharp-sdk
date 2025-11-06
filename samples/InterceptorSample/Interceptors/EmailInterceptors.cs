using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InterceptorSample.Interceptors;

/// <summary>
/// Demonstrates interceptors for email redaction, validation, and observability.
/// </summary>
[McpServerInterceptorType]
public partial class EmailInterceptors
{
    // Regex to match email addresses (simplified pattern for demonstration)
    [GeneratedRegex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Mutation interceptor that redacts email addresses in payloads.
    /// Runs with priority 10 (lower numbers run first).
    /// </summary>
    [McpServerInterceptor(
        Id = "email-redactor",
        Name = "Email Redaction Interceptor",
        Type = InterceptorType.Mutation,
        Priority = 10)]
    [Description("Redacts email addresses from request and response payloads by replacing them with [REDACTED_EMAIL].")]
    public static InvokeInterceptorResult RedactEmails(InvokeInterceptorRequestParams requestParams)
    {
        if (requestParams.Payload is not { } payload)
        {
            return new InvokeInterceptorResult();
        }

        // Convert JsonElement to string, redact emails, convert back
        string payloadString = payload.GetRawText();
        string redactedString = EmailRegex().Replace(payloadString, "[REDACTED_EMAIL]");

        // Parse back to JsonElement
        var redactedPayload = JsonDocument.Parse(redactedString).RootElement;

        return new InvokeInterceptorResult
        {
            ModifiedPayload = redactedPayload,
            Metadata = new Dictionary<string, JsonElement>
            {
                ["interceptor"] = JsonSerializer.SerializeToElement("email-redactor"),
                ["redacted"] = JsonSerializer.SerializeToElement(payloadString != redactedString)
            }
        };
    }

    /// <summary>
    /// Validation interceptor that checks for potentially leaked email addresses.
    /// Runs with priority 5 (runs before the mutation interceptor).
    /// </summary>
    [McpServerInterceptor(
        Id = "email-validator",
        Name = "Email Leak Validator",
        Type = InterceptorType.Validation,
        Priority = 5,
        Phases = new[] { InterceptorPhase.Response })]  // Only validate responses
    [Description("Validates that response payloads don't contain unredacted email addresses.")]
    public static InvokeInterceptorResult ValidateNoEmails(InvokeInterceptorRequestParams requestParams)
    {
        if (requestParams.Payload is not { } payload)
        {
            return new InvokeInterceptorResult();
        }

        string payloadString = payload.GetRawText();
        var matches = EmailRegex().Matches(payloadString);

        if (matches.Count > 0)
        {
            var validationResults = new List<ValidationResult>();
            foreach (Match match in matches)
            {
                validationResults.Add(new ValidationResult
                {
                    Severity = ValidationSeverity.Warning,
                    Message = $"Found potentially unredacted email: {match.Value}",
                    Path = "$.payload"
                });
            }

            return new InvokeInterceptorResult
            {
                ValidationResults = validationResults.ToArray()
            };
        }

        return new InvokeInterceptorResult
        {
            ValidationResults = Array.Empty<ValidationResult>()
        };
    }

    /// <summary>
    /// Observability interceptor that logs interceptor execution.
    /// Runs with priority 1 (runs first, before validation and mutation).
    /// </summary>
    [McpServerInterceptor(
        Id = "request-logger",
        Name = "Request Logger",
        Type = InterceptorType.Observability,
        Priority = 1)]
    [Description("Logs when interceptors are invoked for observability.")]
    public static InvokeInterceptorResult LogRequest(
        InvokeInterceptorRequestParams requestParams,
        ILogger<EmailInterceptors> logger)
    {
        logger.LogInformation(
            "Interceptor invoked for event: {Event}, phase: {Phase}",
            requestParams.Event,
            requestParams.Phase);

        return new InvokeInterceptorResult
        {
            Metadata = new Dictionary<string, JsonElement>
            {
                ["interceptor"] = JsonSerializer.SerializeToElement("request-logger"),
                ["timestamp"] = JsonSerializer.SerializeToElement(DateTime.UtcNow)
            }
        };
    }
}
