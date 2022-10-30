using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Services.FileService;

public class SchemaBaseProperties
{
    [JsonPropertyName("triggers")]
    public object Triggers { get; set; }

    [JsonPropertyName("properties")]
    public object Properties { get; set; }
}