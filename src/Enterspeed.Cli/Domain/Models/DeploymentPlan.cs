using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Domain.Models
{
    public class DeploymentPlan
    {
        public SchemaDeployment[] Schemas { get; set; }

    }

    public class SchemaDeployment
    {
        [JsonPropertyName("Schema")]
        public string SchemaAlias { get; set; }
        public int Version { get; set; }
    }
}
