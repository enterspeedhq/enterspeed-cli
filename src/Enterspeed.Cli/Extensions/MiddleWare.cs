using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Services.ConsoleOutput;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enterspeed.Cli.Extensions
{
    public class MiddleWare
    {
        public static Action<InvocationContext> SetGlobalOptions(
            Option<string> apiKeyOption, 
            Option<OutputStyle> outPutStyle,
            Option<string> customEndpointOption)
        {
            return (context) =>
            {
                var apiKeyValue = context.ParseResult.GetValueForOption(apiKeyOption);
                var outPutStyleValue = context.ParseResult.GetValueForOption(outPutStyle);
                var customEndpoint = context.ParseResult.GetValueForOption(customEndpointOption);

                var host = context.BindingContext.GetService<IHost>();
                var globalOptions = host?.Services.GetService<GlobalOptions>();

                globalOptions?.Set(apiKeyValue, outPutStyleValue, customEndpoint);
            };
        }
    }
}