using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterspeed.Cli.Extensions
{
    public  class MiddleWare
    {
        public static Action<InvocationContext> ApiKey(Option<string> apiKeyOption)
        {
            return (context) =>
            {
                var apiKeyValue = context.ParseResult.GetValueForOption(apiKeyOption);
                if (!string.IsNullOrEmpty(apiKeyValue))
                {
                    return;
                }

                var apiKey = context.BindingContext.GetService<ApiKey>();
                apiKey?.Set(apiKeyValue);
            };
        }
    }
}