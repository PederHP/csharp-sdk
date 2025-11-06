namespace ModelContextProtocol.Server;

/// <summary>
/// Used to attribute a type containing methods that should be exposed as <see cref="McpServerInterceptor"/>s.
/// </summary>
/// <remarks>
/// <para>
/// This attribute is used to mark a class containing methods that should be automatically
/// discovered and registered as <see cref="McpServerInterceptor"/>s. When combined with discovery methods like
/// WithInterceptorsFromAssembly, it enables automatic registration of interceptors without explicitly listing each interceptor
/// class. The attribute is not necessary when a reference to the type is provided directly to a method like WithInterceptors.
/// </para>
/// <para>
/// Within a class marked with this attribute, individual methods that should be exposed as
/// interceptors must be marked with the <see cref="McpServerInterceptorAttribute"/>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class McpServerInterceptorTypeAttribute : Attribute;
