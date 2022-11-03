using System.Text;
using System.Text.Json;
using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService;

public class FileService : IFileService
{
    private const string SchemaDirectory = "schemas";
    public const string DefaultDeploymentPlanFileName = "deploymentplan.json";

    private static bool SchemaFolderExist => Directory.Exists(SchemaDirectory);
    private static bool DeploymentPlanExist => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DefaultDeploymentPlanFileName));


    public void CreateSchema(string alias, int version)
    {
        if (!SchemaFolderExist)
        {
            Directory.CreateDirectory(SchemaDirectory);
        }

        using (var fs = File.Create(GetSchemaFilePath(alias)))
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(new SchemaBaseProperties()
            {
                Properties = new object(),
                Triggers = new object()
            });

            fs.Write(jsonBytes, 0, jsonBytes.Length);
        }
    }

    public SchemaBaseProperties GetSchema(string alias, string filePath = null)
    {
        var schemaFilePath = filePath ?? GetSchemaFilePath(alias);
        var schemaFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), schemaFilePath));
        var schemaProperties = JsonSerializer.Deserialize<SchemaBaseProperties>(schemaFile);
        return schemaProperties;
    }

    public DeploymentPlanProperties GetDeploymentPlan(string filename = DefaultDeploymentPlanFileName)
    {
        if (!DeploymentPlanExist) return null;
        var deploymentPlanFile = File.ReadAllText(filename);
        return JsonSerializer.Deserialize<DeploymentPlanProperties>(deploymentPlanFile);
    }

    private string GetSchemaFilePath(string alias)
    {
        return $"{SchemaDirectory}/{alias}.json";
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

        var deploymentPlanFile = File.ReadAllText(DefaultDeploymentPlanFileName);
        var deploymentPlanProperties = JsonSerializer.Deserialize<DeploymentPlanProperties>(deploymentPlanFile);
        return deploymentPlanProperties;
    }

    private void UpdateDeploymentPlan(DeploymentPlanProperties deploymentPlanProperties)
    {
        if (DeploymentPlanExist)
        {
            DeleteDeploymentPlan();
        }

        using (var fs = File.Create(DefaultDeploymentPlanFileName))
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(deploymentPlanProperties);
            fs.Write(jsonBytes, 0, jsonBytes.Length);
        }
    }

    private void DeleteDeploymentPlan()
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
}