using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Services.FileService.Models;

public class SchemaBaseProperties
{
    [JsonPropertyName("triggers")]
    public object Triggers { get; set; }

    [JsonPropertyName("route")]
    public object Route { get; set; }

    [JsonPropertyName("properties")]
    public object Properties { get; set; }
}