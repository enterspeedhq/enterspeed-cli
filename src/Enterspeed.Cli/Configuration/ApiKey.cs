namespace Enterspeed.Cli.Configuration
{
    public class ApiKey
    {
        private static string _apiKey { get; set; }

        public static void Set(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static string Get()
        {
            return _apiKey;
        }
    }
}