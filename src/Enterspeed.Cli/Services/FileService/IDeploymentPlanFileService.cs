using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService
{
    public interface IDeploymentPlanFileService
    {
        DeploymentPlanProperties GetDeploymentPlan(string filename = "deploymentplan.json");
        void UpdateDeploymentPlan(string schemaAlias, int version);
    }
}