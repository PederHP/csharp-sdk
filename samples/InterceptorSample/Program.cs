using InterceptorSample.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// This sample demonstrates how to use interceptors to validate, transform, and observe MCP operations.
// Run with: dotnet run

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithInterceptors<EmailInterceptors>(); // Register interceptors from the EmailInterceptors class

// Configure logging to stderr so it doesn't interfere with MCP stdio communication
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

await builder.Build().RunAsync();
