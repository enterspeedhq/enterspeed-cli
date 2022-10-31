using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Services.FileService.Models;

public class DeploymentPlanSchema
{
    public DeploymentPlanSchema(string schema, int version)
    {
        Schema = schema;
        Version = version;
    }

    [JsonPropertyName("schema")]
    public string Schema { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }
}