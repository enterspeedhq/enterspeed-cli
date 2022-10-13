using System.Text.Json;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Domain.JsonConverters;

namespace Enterspeed.Cli.Services.ConsoleOutput;

public class OutputService : IOutputService
{
    private readonly GlobalOptions _globalOptions;

    public OutputService(GlobalOptions globalOptions)
    {
        _globalOptions = globalOptions;
    }

    public void Write<T>(T value)
    {
        if (_globalOptions.OutPutStyle == OutputStyle.Table)
        {
            WriteTable(value);
        }
        else
        {
            WriteJson(value);
        }
    }

    private void WriteJson<T>(T value)
    {
        if (value == null)
        {
            return;
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        jsonSerializerOptions.Converters.Add(new TenantIdJsonConverter());

        var jsonString = JsonSerializer.Serialize(value, jsonSerializerOptions);
        Console.WriteLine(jsonString);
    }

    private void WriteTable<T>(T value)
    {
        Console.WriteLine("TABLE output not implemented yet");
    }
}