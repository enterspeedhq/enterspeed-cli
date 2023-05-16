using Enterspeed.Cli.Configuration;
using Microsoft.Extensions.Configuration;

namespace Enterspeed.Cli.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly Settings _settings;

        public SettingsService(IConfiguration configuration)
        {
            _settings = configuration.GetSection("Settings").Get<Settings>() ?? new Settings();
        }

        public Settings GetSettings() => _settings;

    }
}
