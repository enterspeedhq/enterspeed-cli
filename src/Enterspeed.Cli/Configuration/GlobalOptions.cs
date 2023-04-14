using Enterspeed.Cli.Domain.Exceptions;
using Enterspeed.Cli.Services.ConsoleOutput;

namespace Enterspeed.Cli.Configuration
{
    public class GlobalOptions
    {
        public string ApiKeyValue { get; private set; }
        public OutputStyle OutPutStyle { get; private set; }
        public Uri CustomEndpoint { get; private set; }

        public void Set(string apiKeyValue, OutputStyle outPutStyle, string customEndpoint)
        {
            if (!string.IsNullOrEmpty(apiKeyValue))
            {
                ApiKeyValue = apiKeyValue;
            }

            if (!string.IsNullOrEmpty(customEndpoint))
            {
                if (ApiKeyValue == null)
                {
                    throw new Exception("--customEndpoint option can only be used when --apiKey is set");
                }

                var uri = new Uri(customEndpoint);

                if (uri.Scheme is "http" or "https")
                {
                    CustomEndpoint = uri;
                }
                else
                {
                    throw new UriFormatException("Url scheme must be http or https");
                }
               
            }

            OutPutStyle = outPutStyle;
        }
    }
}