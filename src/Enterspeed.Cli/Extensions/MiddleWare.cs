using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enterspeed.Cli.Extensions
{
    public class MiddleWare
    {
        public static Action<InvocationContext> ApiKey(Option<string> apiKeyOption)
        {
            return (context) =>
            {
                var apiKeyValue = context.ParseResult.GetValueForOption(apiKeyOption);

                if (string.IsNullOrEmpty(apiKeyValue))
                {
                    return;
                }

                var host = context.BindingContext.GetService<IHost>();

                var apiKey = host?.Services.GetService<ApiKey>();
                apiKey?.Set(apiKeyValue);
            };
        }
    }
}