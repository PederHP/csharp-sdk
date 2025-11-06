using ModelContextProtocol.Protocol;
using System.ComponentModel;
using System.Text.Json;

namespace ModelContextProtocol.Server;

/// <summary>
/// Provides options for controlling the creation of an <see cref="McpServerInterceptor"/>.
/// </summary>
/// <remarks>
/// <para>
/// These options allow for customizing the behavior and metadata of interceptors created with
/// <see cref="M:McpServerInterceptor.Create"/>. They provide control over naming, description,
/// interceptor properties, and dependency injection integration.
/// </para>
/// <para>
/// When creating interceptors programmatically rather than using attributes, these options
/// provide the same level of configuration flexibility.
/// </para>
/// </remarks>
public sealed class McpServerInterceptorCreateOptions
{
    /// <summary>
    /// Gets or sets optional services used in the construction of the <see cref="McpServerInterceptor"/>.
    /// </summary>
    /// <remarks>
    /// These services will be used to determine which parameters should be satisfied from dependency injection. As such,
    /// what services are satisfied via this provider should match what's satisfied via the provider passed in at invocation time.
    /// </remarks>
    public IServiceProvider? Services { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier to use for the <see cref="McpServerInterceptor"/>.
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, but an <see cref="McpServerInterceptorAttribute"/> is applied to the method,
    /// the ID from the attribute will be used. If that's not present, an ID based on the method's name will be used.
    /// </remarks>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the human-readable name to use for the <see cref="McpServerInterceptor"/>.
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, but an <see cref="McpServerInterceptorAttribute"/> is applied to the method,
    /// the name from the attribute will be used. If that's not present, the ID will be used.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or set the description to use for the <see cref="McpServerInterceptor"/>.
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, but a <see cref="DescriptionAttribute"/> is applied to the method,
    /// the description from that attribute will be used.
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of interceptor (validation, mutation, or observability).
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, the type must be specified via the <see cref="McpServerInterceptorAttribute"/>.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets the <see cref="JsonSerializerOptions"/> to use for marshaling data to and from the JSON payload.
    /// </summary>
    /// <remarks>
    /// If <see langword="null"/>, <see cref="McpJsonUtilities.DefaultOptions"/> will be used.
    /// </remarks>
    public JsonSerializerOptions? SerializerOptions { get; set; }
}
