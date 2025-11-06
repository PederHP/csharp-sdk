using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the interceptors capability that a server may support.
/// </summary>
/// <remarks>
/// <para>
/// When a server advertises the interceptors capability, it indicates that it can provide
/// interceptors for validation, mutation, and observability of MCP operations.
/// </para>
/// <para>
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </para>
/// </remarks>
public sealed class InterceptorsCapability
{
    /// <summary>
    /// Gets or sets whether the server will send <see cref="NotificationMethods.InterceptorListChangedNotification"/>
    /// notifications when the list of available interceptors changes.
    /// </summary>
    [JsonPropertyName("listChanged")]
    public bool? ListChanged { get; set; }
}
