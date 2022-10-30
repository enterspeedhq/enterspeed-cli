using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Services.FileService;

public class DeploymentPlanProperties
{
    [JsonPropertyName("schemas")]
    public List<DeploymentPlanSchema> Schemas { get; set; }
}