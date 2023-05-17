using Enterspeed.Cli.Configuration;

namespace Enterspeed.Cli.Services;

public interface ISettingsService
{
    Settings GetSettings();
}