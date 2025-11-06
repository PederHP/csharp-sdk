using System.Text.Json.Serialization;

namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the result of a <see cref="RequestMethods.InterceptorsList"/> request,
/// containing a list of available interceptors from the server.
/// </summary>
/// <remarks>
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </remarks>
public sealed class ListInterceptorsResult : PaginatedResult
{
    /// <summary>
    /// Gets or sets the array of interceptors available from the server.
    /// </summary>
    [JsonPropertyName("interceptors")]
    [JsonRequired]
    public required Interceptor[] Interceptors { get; set; }
}
