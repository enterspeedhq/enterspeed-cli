using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using Enterspeed.Cli.Common.Behaviours;
using Enterspeed.Cli.Services.StateService;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Enterspeed.Cli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCli(this IServiceCollection services)
    {
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddSingleton<IStateService, StateService>();
        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        Log.Logger = CreateLogger(services);
        return services;
    }

    private static Serilog.Core.Logger CreateLogger(IServiceCollection services)
    {
        var scope = services.BuildServiceProvider();
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(scope.GetRequiredService<IConfiguration>());

        loggerConfiguration.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);

        return loggerConfiguration.CreateLogger();
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient<IEnterspeedClient, EnterspeedClient>();
        services.AddTransient<IOutputService, OutputService>();
        return services;
    }
}