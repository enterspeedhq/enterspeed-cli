using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Enterspeed.Cli.Commands.Domain;
using Enterspeed.Cli.Commands.Environment;
using Enterspeed.Cli.Commands.EnvironmentClient;
using Enterspeed.Cli.Commands.Login;
using Enterspeed.Cli.Commands.SourceEntity;
using Enterspeed.Cli.Commands.SourceGroup;
using Enterspeed.Cli.Commands.Tenant;
using Enterspeed.Cli.Commands.View;
using Enterspeed.Cli.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Enterspeed.Cli;

internal class Program
{
    private static readonly Option<string> ApiKeyOption = new("--apiKey");

    internal static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);

    public static async Task<int> Main(string[] args)
    {
        var runner = BuildCommandLine()
            .UseHost(_ => CreateHostBuilder(args), (builder) => builder
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSerilog();
                    services.AddCli();
                    services.AddApplication();
                })
                .UseCommands()
            )
            .AddMiddleware(MiddleWare.ApiKey(ApiKeyOption))
            .UseDefaults()
            .Build();

        return await runner.InvokeAsync(args);
    }

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand();
        root.AddCommand(new LoginCommand());
        root.AddCommand(TenantCommands.BuildCommands());
        root.AddCommand(EnvironmentCommands.BuildCommands());
        root.AddCommand(EnvironmentClientCommands.BuildCommands());
        root.AddCommand(DomainCommands.BuildCommands());
        root.AddCommand(SourceGroupCommands.BuildCommands());
        root.AddCommand(ViewCommands.BuildCommands());
        root.AddCommand(SourceEntityCommands.BuildCommands());

        root.AddGlobalOption(new Option<OutputStyle>(new[] { "--output", "-o" }, "Set output to json or table") { IsHidden = true });
        root.AddGlobalOption(ApiKeyOption);

        return new CommandLineBuilder(root);
    }
}