﻿using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.EnterspeedClient;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Enterspeed.Cli.Services.StateService;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Exceptions;
using System.CommandLine;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Hosting;
using Serilog.Events;

namespace Enterspeed.Cli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCli(this IServiceCollection services)
    {
        services.AddSingleton<IStateService, StateService>();
        return services;
    }

    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration loggerConfiguration, HostBuilderContext context, Option<bool> verboseLogging)
    {
        var verbose = context.GetInvocationContext().ParseResult.GetValueForOption(verboseLogging);
        var logEventLevel = verbose ? LogEventLevel.Verbose : LogEventLevel.Warning;

        loggerConfiguration.MinimumLevel.Override("Microsoft", logEventLevel);
        loggerConfiguration.WriteTo.File(new CompactJsonFormatter(), "log.json", logEventLevel);
        loggerConfiguration.WriteTo.Console(logEventLevel);
        loggerConfiguration.Enrich.WithExceptionDetails();

        return loggerConfiguration;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient<IEnterspeedClient, EnterspeedClient>();
        services.AddTransient<IOutputService, OutputService>();
        services.AddTransient<ISchemaFileService, SchemaFileService>();
        services.AddTransient<IDeploymentPlanFileService, DedploymentPlanFileService>();
        services.AddSingleton<GlobalOptions>();
        return services;
    }
}