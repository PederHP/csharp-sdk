using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using System.Reflection;

namespace ModelContextProtocol.Server;

/// <summary>
/// Represents an invocable interceptor used by Model Context Protocol servers.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="McpServerInterceptor"/> is an abstract base class that represents an MCP interceptor for use in the server.
/// Instances of <see cref="McpServerInterceptor"/> can be added into a <see cref="IServiceCollection"/> to be picked up
/// automatically when <see cref="McpServer"/> is used to create an <see cref="McpServer"/>, or added into a
/// <see cref="McpServerPrimitiveCollection{McpServerInterceptor}"/>.
/// </para>
/// <para>
/// Most commonly, <see cref="McpServerInterceptor"/> instances are created using the static <see cref="M:McpServerInterceptor.Create"/> methods.
/// These methods enable creating an <see cref="McpServerInterceptor"/> for a method, specified via a <see cref="Delegate"/> or
/// <see cref="MethodInfo"/>, and are what are used implicitly by WithInterceptorsFromAssembly and WithInterceptors extensions.
/// </para>
/// <para>
/// Interceptor methods can accept the following special parameters that are automatically bound:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <see cref="CancellationToken"/> parameters are automatically bound to a <see cref="CancellationToken"/> provided by the
///       <see cref="McpServer"/> and that respects any <see cref="CancelledNotificationParams"/>s sent by the client for this operation's
///       <see cref="RequestId"/>.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="IServiceProvider"/> parameters are bound from the <see cref="RequestContext{InvokeInterceptorRequestParams}"/> for this request.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="McpServer"/> parameters are bound directly to the <see cref="McpServer"/> instance associated
///       with this request's <see cref="RequestContext{InvokeInterceptorRequestParams}"/>. Such parameters may be used to understand
///       what server is being used to process the request, and to interact with the client issuing the request to that server.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="IProgress{ProgressNotificationValue}"/> parameters accepting <see cref="ProgressNotificationValue"/> values
///       are bound to an <see cref="IProgress{ProgressNotificationValue}"/> instance manufactured to forward progress notifications
///       from the interceptor to the client. If the client included a <see cref="ProgressToken"/> in their request, progress reports issued
///       to this instance will propagate to the client as <see cref="NotificationMethods.ProgressNotification"/> notifications with
///       that token. If the client did not include a <see cref="ProgressToken"/>, the instance will ignore any progress reports issued to it.
///     </description>
///   </item>
///   <item>
///     <description>
///       When the <see cref="McpServerInterceptor"/> is constructed, it may be passed an <see cref="IServiceProvider"/> via
///       <see cref="McpServerInterceptorCreateOptions.Services"/>. Any parameter that can be satisfied by that <see cref="IServiceProvider"/>
///       according to <see cref="IServiceProviderIsService"/> will be resolved from the <see cref="IServiceProvider"/> provided to
///       <see cref="InvokeAsync"/> rather than from the payload.
///     </description>
///   </item>
///   <item>
///     <description>
///       Any parameter attributed with <see cref="FromKeyedServicesAttribute"/> will similarly be resolved from the
///       <see cref="IServiceProvider"/> provided to <see cref="InvokeAsync"/> rather than from the payload.
///     </description>
///   </item>
/// </list>
/// <para>
/// All other parameters are deserialized from the <see cref="InvokeInterceptorRequestParams.Payload"/> JSON element.
/// </para>
/// </remarks>
public abstract class McpServerInterceptor : IMcpServerPrimitive
{
    /// <summary>Initializes a new instance of the <see cref="McpServerInterceptor"/> class.</summary>
    protected McpServerInterceptor()
    {
    }

    /// <summary>Gets the protocol <see cref="Interceptor"/> type for this instance.</summary>
    public abstract Interceptor ProtocolInterceptor { get; }

    /// <summary>
    /// Gets the metadata for this interceptor instance.
    /// </summary>
    /// <remarks>
    /// Contains attributes from the associated MethodInfo and declaring class (if any),
    /// with class-level attributes appearing before method-level attributes.
    /// </remarks>
    public abstract IReadOnlyList<object> Metadata { get; }

    /// <summary>Invokes the <see cref="McpServerInterceptor"/>.</summary>
    /// <param name="request">The request information resulting in the invocation of this interceptor.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The result from invoking the interceptor.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public abstract ValueTask<InvokeInterceptorResult> InvokeAsync(
        RequestContext<InvokeInterceptorRequestParams> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via a <see cref="Delegate"/> instance.
    /// </summary>
    /// <param name="method">The method to be represented via the created <see cref="McpServerInterceptor"/>.</param>
    /// <param name="options">Optional options used in the creation of the <see cref="McpServerInterceptor"/> to control its behavior.</param>
    /// <returns>The created <see cref="McpServerInterceptor"/> for invoking <paramref name="method"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    public static McpServerInterceptor Create(
        Delegate method,
        McpServerInterceptorCreateOptions? options = null) =>
        AIFunctionMcpServerInterceptor.Create(method, options);

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via a <see cref="MethodInfo"/> instance.
    /// </summary>
    /// <param name="method">The method to be represented via the created <see cref="McpServerInterceptor"/>.</param>
    /// <param name="target">The instance if <paramref name="method"/> is an instance method; otherwise, <see langword="null"/>.</param>
    /// <param name="options">Optional options used in the creation of the <see cref="McpServerInterceptor"/> to control its behavior.</param>
    /// <returns>The created <see cref="McpServerInterceptor"/> for invoking <paramref name="method"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="method"/> is an instance method but <paramref name="target"/> is <see langword="null"/>.</exception>
    public static McpServerInterceptor Create(
        MethodInfo method,
        object? target = null,
        McpServerInterceptorCreateOptions? options = null) =>
        AIFunctionMcpServerInterceptor.Create(method, target, options);

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via an <see cref="MethodInfo"/> for
    /// an instance method, along with a factory function to create the target object on each invocation.
    /// </summary>
    /// <param name="method">The instance method to be represented via the created <see cref="McpServerInterceptor"/>.</param>
    /// <param name="createTargetFunc">
    /// Callback used on each function invocation to create an instance of the type on which the instance method <paramref name="method"/>
    /// will be invoked. If the returned instance is <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>, it will
    /// be disposed of after the method completes its invocation.
    /// </param>
    /// <param name="options">Optional options used in the creation of the <see cref="McpServerInterceptor"/> to control its behavior.</param>
    /// <returns>The created <see cref="McpServerInterceptor"/> for invoking <paramref name="method"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    public static McpServerInterceptor Create(
        MethodInfo method,
        Func<RequestContext<InvokeInterceptorRequestParams>, object> createTargetFunc,
        McpServerInterceptorCreateOptions? options = null) =>
        AIFunctionMcpServerInterceptor.Create(method, createTargetFunc, options);

    /// <inheritdoc />
    public override string ToString() => ProtocolInterceptor.Id;

    /// <inheritdoc />
    string IMcpServerPrimitive.Id => ProtocolInterceptor.Id;
}
