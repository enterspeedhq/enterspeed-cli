using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Enterspeed.Cli.Commands.Deploy;
using Enterspeed.Cli.Commands.Domain;
using Enterspeed.Cli.Commands.Environment;
using Enterspeed.Cli.Commands.EnvironmentClient;
using Enterspeed.Cli.Commands.Login;
using Enterspeed.Cli.Commands.Schema;
using Enterspeed.Cli.Commands.Source;
using Enterspeed.Cli.Commands.SourceEntity;
using Enterspeed.Cli.Commands.SourceGroup;
using Enterspeed.Cli.Commands.Tenant;
using Enterspeed.Cli.Commands.View;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Enterspeed.Cli
{
    internal class Program
    {
        private static readonly Option<string> ApiKeyOption = new("--apiKey");

        private static readonly Option<OutputStyle> OutPutStyle = new(new[] { "--output", "-o" }, "Set output to json or table")
        {
            IsHidden = true
        };

        private static readonly Option<string> CustomEndpointOption = new("--customEndpoint")
        {
            IsHidden = true
        };

        private static readonly Option<bool> VerboseLogging = new(new[] { "--verbose", "-v" }, "verbose");

        internal static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);

        public static async Task<int> Main(string[] args)
        {
            var runner = BuildCommandLine()
                .UseHost(_ => CreateHostBuilder(args), (builder) => builder
                    .ConfigureAppConfiguration((context, configuration) =>
                    {
                        configuration.AddJsonFile($"appsettings.local.json", optional: true);
                    })
                    .ConfigureServices((_, services) =>
                    {
                        services.AddCli();
                        services.AddApplication();
                    })
                    .UseSerilog((context, loggerConfiguration) => loggerConfiguration.ConfigureSerilog(context, VerboseLogging))
                    .UseCommands())
                .AddMiddleware(MiddleWare.SetGlobalOptions(ApiKeyOption, OutPutStyle, CustomEndpointOption))
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
            root.AddCommand(SourceCommands.BuildCommands());
            root.AddCommand(ViewCommands.BuildCommands());
            root.AddCommand(SourceEntityCommands.BuildCommands());
            root.AddCommand(SchemaCommands.BuildCommands());
            root.AddCommand(DeployCommands.BuildCommands());

            root.AddGlobalOption(OutPutStyle);
            root.AddGlobalOption(VerboseLogging);
            root.AddGlobalOption(ApiKeyOption);
            root.AddGlobalOption(CustomEndpointOption);

            return new CommandLineBuilder(root);
        }
    }
}