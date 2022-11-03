using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService;

public interface IFileService
{
    void CreateSchema(string alias, int version);
    SchemaBaseProperties GetSchema(string alias, string filePath = null);
    DeploymentPlanProperties GetDeploymentPlan(string filename = FileService.DefaultDeploymentPlanFileName);
}