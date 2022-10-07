namespace Enterspeed.Cli.Configuration
{
    public class ApiKey
    {
        public string Value { get; private set; }

        public void Set(string value)
        {
            Value = value;
        }
    }
}