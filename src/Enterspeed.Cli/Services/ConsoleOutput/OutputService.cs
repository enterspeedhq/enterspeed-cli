using System.Text.Json;
using Enterspeed.Cli.Domain.JsonConverters;

namespace Enterspeed.Cli.Services.ConsoleOutput;

public class OutputService : IOutputService
{
    public OutputService()
    {
            
    }

    public void Write<T>(T value, OutputStyle style = OutputStyle.Json)
    {
        switch (style)
        {
            case OutputStyle.Json:
                WriteJson(value);
                return;
            case OutputStyle.Table:
                WriteTable(value);
                return;
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