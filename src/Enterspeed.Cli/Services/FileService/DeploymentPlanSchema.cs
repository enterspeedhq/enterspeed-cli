using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Services.FileService;

public class DeploymentPlanSchema
{
    public DeploymentPlanSchema(string schema, int version, string mappingSchemaGuid)
    {
        Schema = schema;
        Version = version;
        MappingSchemaGuid = mappingSchemaGuid;
    }

    [JsonPropertyName("schema")]
    public string Schema { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("mappingSchemaGuid")]
    public string MappingSchemaGuid { get; set; }
}