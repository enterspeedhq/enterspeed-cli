using Enterspeed.Cli.Services.FileService.Models;
using System.Text.Json;

namespace Enterspeed.Cli.Services.FileService
{
    public class DeploymentPlanFileService : IDeploymentPlanFileService
    {
        public const string DefaultDeploymentPlanFileName = "deploymentplan.json";

        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        private bool DeploymentPlanExist => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DefaultDeploymentPlanFileName));

        public void UpdateDeploymentPlan(string schemaAlias, int version)
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

            var deploymentPlanFile = File.ReadAllText(DefaultDeploymentPlanFileName);
            return JsonSerializer.Deserialize<DeploymentPlanProperties>(deploymentPlanFile);
        }

        private void UpdateDeploymentPlan(DeploymentPlanProperties deploymentPlanProperties)
        {
            if (DeploymentPlanExist)
            {
                DeleteDeploymentPlan();
            }

            using (var fs = File.Create(DefaultDeploymentPlanFileName))
            {
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(deploymentPlanProperties, SerializerOptions);
                fs.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        public void DeleteDeploymentPlan()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), DefaultDeploymentPlanFileName));
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

        public DeploymentPlanProperties GetDeploymentPlan(string filename = DefaultDeploymentPlanFileName)
        {
            if (!DeploymentPlanExist) return null;
            var deploymentPlanFile = File.ReadAllText(filename);

            return JsonSerializer.Deserialize<DeploymentPlanProperties>(deploymentPlanFile);
        }
    }
}
