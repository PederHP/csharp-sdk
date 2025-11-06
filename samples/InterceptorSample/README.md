# Interceptor Sample

This sample demonstrates how to use MCP interceptors for validation, mutation, and observability of protocol operations.

## Overview

The sample includes three types of interceptors:

1. **Email Redaction (Mutation)** - Automatically redacts email addresses from payloads
2. **Email Validation (Validation)** - Checks for potentially leaked emails in responses
3. **Request Logging (Observability)** - Logs when interceptors are invoked

## Interceptor Types

### Mutation Interceptors
Transform message content sequentially by priority. In this sample, the email redaction interceptor replaces any email addresses with `[REDACTED_EMAIL]`.

```csharp
[McpServerInterceptor(
    Id = "email-redactor",
    Type = InterceptorType.Mutation,
    Priority = 10)]
public static InvokeInterceptorResult RedactEmails(InvokeInterceptorRequestParams requestParams)
{
    // Transform the payload
    return new InvokeInterceptorResult { ModifiedPayload = redactedPayload };
}
```

### Validation Interceptors
Check inputs/outputs and return structured validation results with severity levels (info/warn/error).

```csharp
[McpServerInterceptor(
    Id = "email-validator",
    Type = InterceptorType.Validation,
    Priority = 5,
    Phases = new[] { InterceptorPhase.Response })]
public static InvokeInterceptorResult ValidateNoEmails(InvokeInterceptorRequestParams requestParams)
{
    return new InvokeInterceptorResult
    {
        ValidationResults = validationResults.ToArray()
    };
}
```

### Observability Interceptors
Perform fire-and-forget logging and metrics collection.

```csharp
[McpServerInterceptor(
    Id = "request-logger",
    Type = InterceptorType.Observability,
    Priority = 1)]
public static InvokeInterceptorResult LogRequest(
    InvokeInterceptorRequestParams requestParams,
    ILogger<EmailInterceptors> logger)
{
    logger.LogInformation("Processing: {Event}", requestParams.Event);
    return new InvokeInterceptorResult();
}
```

## Running the Sample

```bash
dotnet run
```

The server will start and wait for MCP client connections over stdio.

## Registration

Interceptors are registered using the builder pattern:

```csharp
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithInterceptors<EmailInterceptors>();  // Register all interceptors in the class
```

You can also use:
- `WithInterceptors<T>()` - Register from a type
- `WithInterceptors(instance)` - Register from an instance
- `WithInterceptorsFromAssembly()` - Scan assembly for `[McpServerInterceptorType]` classes

## Execution Order

Interceptors execute in priority order (lower numbers first), then alphabetically by ID:

1. **Priority 1**: Request Logger (Observability)
2. **Priority 5**: Email Validator (Validation)
3. **Priority 10**: Email Redactor (Mutation)

## Features Demonstrated

- ✅ Attribute-based interceptor registration
- ✅ Multiple interceptor types (Validation, Mutation, Observability)
- ✅ Priority-based execution ordering
- ✅ Phase-aware execution (Request vs Response)
- ✅ Dependency injection (ILogger)
- ✅ Metadata return values
- ✅ Regular expression pattern matching
- ✅ JSON payload transformation

## Learn More

- [SEP-1763: Interceptors](https://github.com/modelcontextprotocol/modelcontextprotocol/issues/1763)
- [Model Context Protocol Documentation](https://modelcontextprotocol.io)
