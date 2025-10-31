# File-Based MCP Server Sample

This sample demonstrates how to create a complete MCP (Model Context Protocol) server using .NET 10's file-based programs feature. Unlike traditional .NET projects that require a `.csproj` file, file-based programs allow you to write and run complete applications in a single `.cs` file.

## What are File-Based Programs?

File-based programs are a .NET 10 feature that enables you to:
- Write complete applications without a project file (`.csproj`)
- Use package and project references via preprocessor directives (`#:package`, `#:project`)
- Run programs directly with `dotnet run <filename>.cs`
- Create self-contained, portable scripts
- Prototype and experiment quickly with minimal overhead

This approach is ideal for:
- Learning and education
- Quick prototyping
- Simple command-line utilities
- Standalone scripts
- Code samples and demonstrations

## Requirements

- .NET 10 SDK (RC2 or later)
- No project file required!

## Running the Sample

Simply run the Program.cs file directly:

```bash
dotnet run Program.cs
```

The server will start and listen for MCP messages on stdin/stdout (stdio transport).

### Making it Executable (Unix/Linux/macOS)

On Unix-like systems, you can make the file executable:

```bash
chmod +x Program.cs
./Program.cs
```

Note: The shebang line uses `/usr/bin/env` to locate `dotnet`, so ensure it's in your PATH.

## Testing the Server

You can test the server by sending JSON-RPC messages to stdin. Here's an example:

### Initialize the server:
```bash
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0"}}}' | dotnet run Program.cs
```

### List available tools:
```bash
(
  echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0"}}}'
  sleep 0.5
  echo '{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}'
  sleep 1
) | dotnet run Program.cs 2>/dev/null | grep '^{' | jq .
```

### Call the echo tool:
```bash
(
  echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0"}}}'
  sleep 0.5
  echo '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"echo","arguments":{"message":"Hello, MCP!"}}}'
  sleep 1
) | dotnet run Program.cs 2>/dev/null | grep '^{' | jq .
```

## Understanding the Code

The `Program.cs` file demonstrates several .NET 10 features:

### Shebang Support
```csharp
#!/usr/bin/env -S dotnet run --
```
Allows the file to be executed directly on Unix-like systems.

### Package Directives
```csharp
#:package Microsoft.Extensions.Hosting
```
Declares NuGet package dependencies without a project file.

### Project Directives
```csharp
#:project ../../src/ModelContextProtocol/ModelContextProtocol.csproj
```
References local projects for development scenarios.

### Top-Level Statements
```csharp
var builder = Host.CreateApplicationBuilder(args);
// ... configure services ...
await builder.Build().RunAsync();
```
No need for a `Main` method or `Program` class.

### File-Scoped Types
```csharp
file class EchoTool { ... }
```
The `file` keyword keeps types scoped to a single file, preventing pollution of the global namespace.

## Features Demonstrated

- **Stdio Transport**: Server communicates via standard input/output
- **MCP Tools**: Implements a simple `echo` tool
- **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection
- **Hosting**: Leverages Microsoft.Extensions.Hosting for application lifecycle
- **Logging**: Configured to output logs to stderr (not stdout, which is used for MCP messages)

## File-Based vs Project-Based: When to Use Each

### Use File-Based Programs When:
- Prototyping or learning MCP
- Building simple, self-contained utilities
- Creating educational samples
- Sharing code snippets that should "just work"
- You need a quick script without project overhead

### Use Project-Based Approach When:
- Building production applications
- Need multiple files organized in folders
- Require complex build configurations
- Publishing NuGet packages
- Working with teams on larger codebases
- Need advanced IDE features like IntelliSense (file-based support is limited)

## Limitations

File-based programs have some limitations:
- Limited IDE support (IntelliSense may not work fully)
- All code must be in a single file (or you can reference other files/projects)
- Debugging support may be limited compared to project-based apps
- Package version specification is limited (uses latest compatible version)

## Reference

- [File-Based Programs Tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/file-based-programs)
- [C# Preprocessor Directives for File-Based Apps](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#file-based-apps)
- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)

## Comparison with Other Samples

This sample provides the same functionality as `QuickstartWeatherServer` or `EverythingServer`, but in a single file without a project structure. Compare with:
- **QuickstartWeatherServer**: Traditional project-based approach with separate tool classes
- **InMemoryTransport**: Shows minimal MCP server setup
- **EverythingServer**: Full-featured server with multiple capabilities

## Contributing

When modifying this sample, ensure:
- The file remains self-contained and runnable with just `dotnet run Program.cs`
- Package and project references are minimal
- Comments explain key concepts for learners
- The example remains simple and focused on education
