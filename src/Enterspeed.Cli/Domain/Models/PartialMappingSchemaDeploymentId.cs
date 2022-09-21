using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class PartialMappingSchemaDeploymentId : Id
{
    public PartialMappingSchemaDeploymentId(string idValue)
        : base(idValue)
    {
    }

    public string DeploymentGuid { get; set; }

    public string PartialMappingSchemaGuid { get; set; }

    public int Version { get; set; }

    public static string From(
        string partialMappingSchemaGuid,
        int partialMappingSchemaVersion,
        string deploymentGuid) => $"{IdBase}PartialMappingSchema/{partialMappingSchemaGuid}/Version/{partialMappingSchemaVersion}/Deployment/{deploymentGuid}";

    public static bool TryParse(
        string partialMappingSchemaDeploymentId,
        out PartialMappingSchemaDeploymentId result)
    {
        try
        {
            result = Parse(partialMappingSchemaDeploymentId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static PartialMappingSchemaDeploymentId Parse(string partialMappingSchemaDeploymentId)
    {
        CheckIdValidity(partialMappingSchemaDeploymentId);
        var gidValues = GetIdValues(partialMappingSchemaDeploymentId);

        ValidateOrder(gidValues, "PartialMappingSchema", "Version", "Deployment");
        var partialMappingSchemaGuidAsString = gidValues.FirstOrDefault(x => x.Key == "PartialMappingSchema").Value;
        var partialMappingSchemaGuid = GetValidatedGuid(partialMappingSchemaGuidAsString);

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        var deploymentGuidAsString = gidValues.FirstOrDefault(x => x.Key == "Deployment").Value;
        var deploymentGuid = GetValidatedGuid(deploymentGuidAsString);

        return new PartialMappingSchemaDeploymentId(
            From(
                partialMappingSchemaGuid.ToString(),
                version,
                deploymentGuid.ToString()))
        {
            PartialMappingSchemaGuid = partialMappingSchemaGuid.ToString(),
            Version = version,
            DeploymentGuid = deploymentGuid.ToString()
        };
    }
}
