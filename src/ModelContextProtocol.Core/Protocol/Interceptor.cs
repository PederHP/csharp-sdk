using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents metadata about an interceptor available from an MCP server.
/// </summary>
/// <remarks>
/// <para>
/// Interceptors provide validation, mutation, and observability capabilities for MCP operations.
/// They can be invoked by clients to process requests and responses according to defined policies.
/// </para>
/// <para>
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </para>
/// </remarks>
public sealed class Interceptor
{
    /// <summary>
    /// Gets or sets the unique identifier for this interceptor.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonRequired]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the human-readable name of the interceptor.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonRequired]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets an optional description of what this interceptor does.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of interceptor (validation, mutation, or observability).
    /// </summary>
    [JsonPropertyName("type")]
    [JsonRequired]
    public required InterceptorType Type { get; set; }

    /// <summary>
    /// Gets or sets the execution priority of this interceptor.
    /// </summary>
    /// <remarks>
    /// Lower numbers execute first. When priorities are equal, interceptors are executed
    /// in alphabetical order by ID for deterministic behavior.
    /// </remarks>
    [JsonPropertyName("priority")]
    [JsonRequired]
    public required int Priority { get; set; }

    /// <summary>
    /// Gets or sets the optional list of event names this interceptor applies to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If null or empty, the interceptor applies to all events.
    /// Event names correspond to MCP protocol methods (e.g., "tools/call", "resources/read").
    /// </para>
    /// </remarks>
    [JsonPropertyName("applicableEvents")]
    public string[]? ApplicableEvents { get; set; }

    /// <summary>
    /// Gets or sets the optional list of phases this interceptor can execute in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If null or empty, the interceptor applies to both request and response phases.
    /// </para>
    /// </remarks>
    [JsonPropertyName("phases")]
    public InterceptorPhase[]? Phases { get; set; }
}
