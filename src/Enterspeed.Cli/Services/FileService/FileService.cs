using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService
{
    public class FileService : IFileService
    {
        private string SchemaDirectory => "schemas";
        private string DeploymentPlanFileName => "deploymentplan.json";
        private bool SchemaFolderExist => Directory.Exists(SchemaDirectory);
        private bool DeploymentPlanExist => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DeploymentPlanFileName));


        public void CreateSchema(string alias, int version)
        {
            if (!SchemaFolderExist)
            {
                Directory.CreateDirectory(SchemaDirectory);
            }

            using (var fs = File.Create(GetSchemaFilePath(alias)))
            {
                var json = JsonSerializer.Serialize(new SchemaBaseProperties()
                {
                    Properties = new object(),
                    Triggers = new object()
                });

                var baseProperties = Encoding.UTF8.GetBytes(json);
                fs.Write(baseProperties, 0, baseProperties.Length);
            }
        }

        public SchemaBaseProperties GetSchema(string alias, string filePath = null)
        {
            var schemaFilePath = filePath ?? GetSchemaFilePath(alias);
            var schemaFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), schemaFilePath));
            var schemaProperties = JsonSerializer.Deserialize<SchemaBaseProperties>(schemaFile);
            return schemaProperties;
        }

        private string GetSchemaFilePath(string alias)
        {
            return SchemaDirectory + "/" + alias + ".json";
        }

        private void UpdateDeploymentPlan(string schemaAlias, int version)
        {
            var deploymentPlanProperties = GetDeploymentPlanProperties();
            MapDeploymentPlanProperties(schemaAlias, version, deploymentPlanProperties);
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
                var json = JsonSerializer.Serialize(deploymentPlanProperties);
                var baseProperties = Encoding.UTF8.GetBytes(json);
                fs.Write(baseProperties, 0, baseProperties.Length);
            }
        }

        private void DeleteDeploymentPlan()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), DeploymentPlanFileName));
        }

        private void MapDeploymentPlanProperties(string schemaAlias, int version, DeploymentPlanProperties deploymentPlanProperties)
        {
            var existingDeploymentPlanSchema = deploymentPlanProperties?.Schemas.FirstOrDefault(dp => dp.Schema == schemaAlias);
            if (existingDeploymentPlanSchema != null)
            {
                existingDeploymentPlanSchema.Version = version;
            }
            else
            {
                deploymentPlanProperties?.Schemas?.Add(new DeploymentPlanSchema(schemaAlias, version));
            }
        }
    }
}