using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Hosting;

namespace Enterspeed.Cli.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseCommands(this IHostBuilder hostBuilder)
    {
        var commandHandlers = typeof(Program).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i == typeof(ICommandHandler))
            );

        foreach (var handler in commandHandlers)
        {
            hostBuilder.UseCommandHandler(handler.DeclaringType, handler);
        }

        return hostBuilder;
    }
}