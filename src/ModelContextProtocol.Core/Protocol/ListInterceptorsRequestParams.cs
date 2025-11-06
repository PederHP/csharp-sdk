namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the parameters used with a <see cref="RequestMethods.InterceptorsList"/> request from a client to request
/// a list of interceptors available from the server.
/// </summary>
/// <remarks>
/// The server responds with a <see cref="ListInterceptorsResult"/> containing the available interceptors.
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </remarks>
public sealed class ListInterceptorsRequestParams : PaginatedRequestParams;
