using System.Text;
using System.Text.Json;

namespace Enterspeed.Cli.Services.FileService
{
    public class FileService : IFileService
    {
        // Should this be in root?
        private string SchemaDirectory => "schemas";
        private string DeploymentPlanFileName => "deploymentplan.json";
        private bool SchemaFolderExist => Directory.Exists(SchemaDirectory);
        private bool DeploymentPlanExist => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DeploymentPlanFileName));

        public void CreateSchema(string schemaAlias, int version, string mappingSchemaGuid)
        {
            if (!SchemaFolderExist)
            {
                Directory.CreateDirectory(SchemaDirectory);
            }

            using (var fs = File.Create(SchemaDirectory + "/" + schemaAlias + ".json"))
            {
                var json = JsonSerializer.Serialize(new SchemaBaseProperties()
                {
                    Properties = new object(),
                    Triggers = new object()
                });

                var baseProperties = Encoding.UTF8.GetBytes(json);
                fs.Write(baseProperties, 0, baseProperties.Length);
            }

            UpdateDeploymentPlan(schemaAlias, version, mappingSchemaGuid);
        }

        private void UpdateDeploymentPlan(string schemaAlias, int version, string mappingSchemaGuid)
        {
            var deploymentPlanProperties = GetDeploymentPlanProperties();
            MapDeploymentPlanProperties(schemaAlias, version, mappingSchemaGuid, deploymentPlanProperties);
            UpdateDeploymentPlan(deploymentPlanProperties);
        }

        private DeploymentPlanProperties GetDeploymentPlanProperties()
        {
            if (!DeploymentPlanExist)
            {
                return new DeploymentPlanProperties
                {
                    Schemas = new List<DeploymentPlanSchema>()
                };
            }

            var deploymentPlanFile = File.ReadAllText(DeploymentPlanFileName);
            var deploymentPlanProperties = JsonSerializer.Deserialize<DeploymentPlanProperties>(deploymentPlanFile);
            return deploymentPlanProperties;
        }

        private void UpdateDeploymentPlan(DeploymentPlanProperties deploymentPlanProperties)
        {
            if (DeploymentPlanExist)
            {
                DeleteDeploymentPlan();
            }

            using (var fs = File.Create(DeploymentPlanFileName))
            {
                var json = JsonSerializer.Serialize(deploymentPlanProperties ?? new DeploymentPlanProperties());
                var baseProperties = Encoding.UTF8.GetBytes(json);
                fs.Write(baseProperties, 0, baseProperties.Length);
            }
        }

        private void DeleteDeploymentPlan()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), DeploymentPlanFileName));
        }

        private void MapDeploymentPlanProperties(string schemaAlias, int version, string mappingSchemaGuid, DeploymentPlanProperties deploymentPlanProperties)
        {
            var existingDeploymentPlanSchema = deploymentPlanProperties?.Schemas.FirstOrDefault(dp => dp.Schema == schemaAlias);
            if (existingDeploymentPlanSchema != null)
            {
                existingDeploymentPlanSchema.Version = version;
                existingDeploymentPlanSchema.MappingSchemaGuid = mappingSchemaGuid;
            }
            else
            {
                deploymentPlanProperties?.Schemas?.Add(new DeploymentPlanSchema(schemaAlias, version, mappingSchemaGuid));
            }
        }
    }
}