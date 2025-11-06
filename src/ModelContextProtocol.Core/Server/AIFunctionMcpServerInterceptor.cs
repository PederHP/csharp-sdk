using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace ModelContextProtocol.Server;

/// <summary>Provides an <see cref="McpServerInterceptor"/> that's implemented via an <see cref="AIFunction"/>.</summary>
internal sealed partial class AIFunctionMcpServerInterceptor : McpServerInterceptor
{
    private readonly AIFunction _function;
    private readonly Interceptor _protocolInterceptor;
    private readonly IReadOnlyList<object> _metadata;
    private readonly JsonSerializerOptions _serializerOptions;

    private AIFunctionMcpServerInterceptor(
        AIFunction function,
        Interceptor protocolInterceptor,
        IReadOnlyList<object> metadata,
        JsonSerializerOptions serializerOptions)
    {
        _function = function;
        _protocolInterceptor = protocolInterceptor;
        _metadata = metadata;
        _serializerOptions = serializerOptions;
    }

    /// <inheritdoc/>
    public override Interceptor ProtocolInterceptor => _protocolInterceptor;

    /// <inheritdoc/>
    public override IReadOnlyList<object> Metadata => _metadata;

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via a <see cref="Delegate"/> instance.
    /// </summary>
    public static new AIFunctionMcpServerInterceptor Create(
        Delegate method,
        McpServerInterceptorCreateOptions? options)
    {
        Throw.IfNull(method);
        options = DeriveOptions(method.Method, options);
        return Create(method.Method, method.Target, options);
    }

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via a <see cref="MethodInfo"/> instance.
    /// </summary>
    public static new AIFunctionMcpServerInterceptor Create(
        MethodInfo method,
        object? target,
        McpServerInterceptorCreateOptions? options)
    {
        Throw.IfNull(method);
        options = DeriveOptions(method, options);

        return Create(
            AIFunctionFactory.Create(method, target, CreateAIFunctionFactoryOptions(method, options)),
            options);
    }

    /// <summary>
    /// Creates an <see cref="McpServerInterceptor"/> instance for a method, specified via a <see cref="MethodInfo"/> instance.
    /// </summary>
    public static new AIFunctionMcpServerInterceptor Create(
        MethodInfo method,
        Func<RequestContext<InvokeInterceptorRequestParams>, object> createTargetFunc,
        McpServerInterceptorCreateOptions? options)
    {
        Throw.IfNull(method);
        Throw.IfNull(createTargetFunc);

        options = DeriveOptions(method, options);

        return Create(
            AIFunctionFactory.Create(method, args =>
            {
                Debug.Assert(args.Services is RequestServiceProvider<InvokeInterceptorRequestParams>,
                    $"The service provider should be a {nameof(RequestServiceProvider<InvokeInterceptorRequestParams>)} for this method to work correctly.");
                return createTargetFunc(((RequestServiceProvider<InvokeInterceptorRequestParams>)args.Services!).Request);
            }, CreateAIFunctionFactoryOptions(method, options)),
            options);
    }

    private static AIFunctionFactoryOptions CreateAIFunctionFactoryOptions(
        MethodInfo method, McpServerInterceptorCreateOptions? options) =>
        new()
        {
            Name = options?.Id ?? method.GetCustomAttribute<McpServerInterceptorAttribute>()?.Id ?? DeriveName(method),
            Description = options?.Description,
            MarshalResult = static (result, _, cancellationToken) => new ValueTask<object?>(result),
            SerializerOptions = options?.SerializerOptions ?? McpJsonUtilities.DefaultOptions,
            ConfigureParameterBinding = pi =>
            {
                if (RequestServiceProvider<InvokeInterceptorRequestParams>.IsAugmentedWith(pi.ParameterType) ||
                    (options?.Services?.GetService<IServiceProviderIsService>() is { } ispis &&
                     ispis.IsService(pi.ParameterType)))
                {
                    return new()
                    {
                        ExcludeFromSchema = true,
                        BindParameter = (pi, args) =>
                            args.Services?.GetService(pi.ParameterType) ??
                            (pi.HasDefaultValue ? null :
                             throw new ArgumentException("No service of the requested type was found.")),
                    };
                }

                if (pi.GetCustomAttribute<FromKeyedServicesAttribute>() is { } keyedAttr)
                {
                    return new()
                    {
                        ExcludeFromSchema = true,
                        BindParameter = (pi, args) => args.Services is not null
                            ? (keyedAttr.Key is not null
                                ? args.Services.GetRequiredKeyedService(pi.ParameterType, keyedAttr.Key)
                                : args.Services.GetRequiredKeyedService(pi.ParameterType, pi.Name!))
                            : pi.HasDefaultValue ? null
                            : throw new ArgumentException("No service of the requested type was found."),
                    };
                }

                return null;
            },
        };

    private static AIFunctionMcpServerInterceptor Create(
        AIFunction function,
        McpServerInterceptorCreateOptions options)
    {
        Throw.IfNull(function);

        var serializerOptions = options.SerializerOptions ?? McpJsonUtilities.DefaultOptions;

        var protocolInterceptor = new Interceptor
        {
            Id = options.Id ?? function.Metadata.Name,
            Name = options.Name ?? options.Id ?? function.Metadata.Name,
            Description = options.Description ?? function.Metadata.Description,
            Type = options.Type ?? throw new InvalidOperationException("Interceptor type must be specified."),
            Priority = options.Priority,
            ApplicableEvents = options.ApplicableEvents,
            Phases = options.Phases
        };

        return new AIFunctionMcpServerInterceptor(
            function,
            protocolInterceptor,
            function.Metadata.AdditionalProperties.TryGetValue("Metadata", out var metadata) && metadata is IReadOnlyList<object> metadataList
                ? metadataList
                : [],
            serializerOptions);
    }

    /// <inheritdoc/>
    public override async ValueTask<InvokeInterceptorResult> InvokeAsync(
        RequestContext<InvokeInterceptorRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        Throw.IfNull(request);

        // Create AIFunctionContext for invocation
        var context = new AIFunctionContext
        {
            Services = new RequestServiceProvider<InvokeInterceptorRequestParams>(request)
        };

        // Deserialize payload to arguments
        var arguments = new Dictionary<string, object?>();
        if (request.Params?.Payload is { } payload)
        {
            // Simple deserialization - in a full implementation, this would match parameter types
            foreach (var param in _function.Metadata.Parameters)
            {
                if (payload.TryGetProperty(param.Name, out var value))
                {
                    arguments[param.Name] = JsonSerializer.Deserialize(value.GetRawText(), param.ParameterType ?? typeof(object), _serializerOptions);
                }
            }
        }

        // Invoke the function
        var result = await _function.InvokeAsync(arguments, context, cancellationToken).ConfigureAwait(false);

        // Convert result based on interceptor type
        return result switch
        {
            InvokeInterceptorResult invokeResult => invokeResult,
            ValidationResult[] validationResults => new InvokeInterceptorResult { ValidationResults = validationResults },
            ValidationResult validationResult => new InvokeInterceptorResult { ValidationResults = [validationResult] },
            JsonElement jsonElement => _protocolInterceptor.Type == InterceptorType.Mutation
                ? new InvokeInterceptorResult { ModifiedPayload = jsonElement }
                : new InvokeInterceptorResult(),
            _ => new InvokeInterceptorResult()
        };
    }

    private static McpServerInterceptorCreateOptions DeriveOptions(
        MethodInfo method,
        McpServerInterceptorCreateOptions? options)
    {
        options ??= new();

        // Extract metadata from attributes
        var attr = method.GetCustomAttribute<McpServerInterceptorAttribute>();
        var descAttr = method.GetCustomAttribute<DescriptionAttribute>();

        options.Id ??= attr?.Id;
        options.Name ??= attr?.Name;
        options.Description ??= options.Description ?? descAttr?.Description;
        options.Type ??= attr?.Type;

        if (attr != null)
        {
            options.Priority = attr.Priority;
            options.ApplicableEvents ??= attr.ApplicableEvents;
            options.Phases ??= attr.Phases;
        }

        // Store metadata for later retrieval
        var metadata = new List<object>();
        if (method.DeclaringType is { } declaringType)
        {
            metadata.AddRange(Attribute.GetCustomAttributes(declaringType));
        }
        metadata.AddRange(Attribute.GetCustomAttributes(method));

        // Add metadata to AIFunction metadata (stored in AdditionalProperties)
        options.SerializerOptions ??= McpJsonUtilities.DefaultOptions;

        return options;
    }

    private static string DeriveName(MethodInfo method)
    {
        string name = method.Name;

        // Remove common async suffix
        if (name.EndsWith("Async", StringComparison.Ordinal))
        {
            name = name.Substring(0, name.Length - "Async".Length);
        }

        // Convert PascalCase to snake_case
        return System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLowerInvariant();
    }
}
