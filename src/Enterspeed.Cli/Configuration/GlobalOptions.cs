namespace Enterspeed.Cli.Configuration
{
    public class GlobalOptions
    {
        public string ApiKeyValue { get; private set; }
        public OutputStyle OutPutStyle { get; private set; }

        public void Set(string apiKeyValue, OutputStyle outPutStyle)
        {
            if (!string.IsNullOrEmpty(apiKeyValue))
            {
                ApiKeyValue = apiKeyValue;
            }

            OutPutStyle = outPutStyle;
        }
    }
}