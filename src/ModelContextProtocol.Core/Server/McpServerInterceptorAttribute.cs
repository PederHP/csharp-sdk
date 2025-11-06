using ModelContextProtocol.Protocol;

namespace ModelContextProtocol.Server;

/// <summary>
/// Used to indicate that a method should be considered an <see cref="McpServerInterceptor"/>.
/// </summary>
/// <remarks>
/// <para>
/// This attribute is applied to methods that should be exposed as interceptors in the Model Context Protocol.
/// When a class containing methods marked with this attribute is registered with McpServerBuilderExtensions,
/// these methods become available as interceptors that can be invoked by MCP clients.
/// </para>
/// <para>
/// When methods are provided directly to <see cref="M:McpServerInterceptor.Create"/>, the attribute is not required.
/// </para>
/// <para>
/// Interceptor methods accept parameters through the <see cref="InvokeInterceptorRequestParams"/> and return
/// results via <see cref="InvokeInterceptorResult"/>. Special parameters like <see cref="CancellationToken"/>,
/// <see cref="IServiceProvider"/>, and <see cref="McpServer"/> are automatically bound and not included in
/// the payload schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class McpServerInterceptorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="McpServerInterceptorAttribute"/> class.
    /// </summary>
    public McpServerInterceptorAttribute()
    {
    }

    /// <summary>Gets or sets the unique identifier of the interceptor.</summary>
    /// <remarks>If <see langword="null"/>, the method name will be used.</remarks>
    public string? Id { get; set; }

    /// <summary>Gets or sets the human-readable name of the interceptor.</summary>
    /// <remarks>If <see langword="null"/>, the ID will be used.</remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of interceptor (validation, mutation, or observability).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.DisallowNull]
    public InterceptorType? Type { get; set; }

    /// <summary>
    /// Gets or sets the execution priority of this interceptor.
    /// </summary>
    /// <remarks>
    /// Lower numbers execute first. When priorities are equal, interceptors are executed
    /// in alphabetical order by ID for deterministic behavior. Default is 0.
    /// </remarks>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the optional list of event names this interceptor applies to.
    /// </summary>
    /// <remarks>
    /// If null or empty, the interceptor applies to all events.
    /// Event names correspond to MCP protocol methods (e.g., "tools/call", "resources/read").
    /// </remarks>
    public string[]? ApplicableEvents { get; set; }

    /// <summary>
    /// Gets or sets the optional list of phases this interceptor can execute in.
    /// </summary>
    /// <remarks>
    /// If null or empty, the interceptor applies to both request and response phases.
    /// </remarks>
    public InterceptorPhase[]? Phases { get; set; }
}
