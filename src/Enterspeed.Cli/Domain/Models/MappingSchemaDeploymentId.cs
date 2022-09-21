using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class MappingSchemaDeploymentId : Id
{
    public MappingSchemaDeploymentId(string idValue)
        : base(idValue)
    {
    }

    public string DeploymentGuid { get; set; }

    public string MappingSchemaGuid { get; set; }

    public string EnvironmentGuid { get; set; }

    public int Version { get; set; }

    public static string From(
        string mappingSchemaGuid,
        int mappingSchemaVersion,
        string environmentGuid,
        string deploymentGuid) =>
        $"{IdBase}MappingSchema/{mappingSchemaGuid}/Version/{mappingSchemaVersion}/Environment/{environmentGuid}/Deployment/{deploymentGuid}";

    public static bool TryParse(string mappingSchemaDeploymentId, out MappingSchemaDeploymentId result)
    {
        try
        {
            result = Parse(mappingSchemaDeploymentId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static MappingSchemaDeploymentId Parse(string mappingSchemaDeploymentId)
    {
        CheckIdValidity(mappingSchemaDeploymentId);
        var gidValues = GetIdValues(mappingSchemaDeploymentId);
        if (!gidValues.ContainsKey("Environment"))
        {
            throw new InvalidIdFormatException("Environment");
        }

        ValidateOrder(gidValues, "MappingSchema", "Version", "Environment", "Deployment");

        var mappingSchemaGuidAsString = gidValues.FirstOrDefault(x => x.Key == "MappingSchema").Value;
        var mappingSchemaGuid = GetValidatedGuid(mappingSchemaGuidAsString);

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        var environmentGuidAsString = gidValues.FirstOrDefault(x => x.Key == "Environment").Value;
        var environmentGuid = GetValidatedGuid(environmentGuidAsString);

        var deploymentGuidAsString = gidValues.FirstOrDefault(x => x.Key == "Deployment").Value;
        var deploymentGuid = GetValidatedGuid(deploymentGuidAsString);

        return new MappingSchemaDeploymentId(
            From(
                mappingSchemaGuid.ToString(),
                version,
                environmentGuid.ToString(),
                deploymentGuid.ToString()))
        {
            MappingSchemaGuid = mappingSchemaGuid.ToString(),
            EnvironmentGuid = environmentGuid.ToString(),
            Version = version,
            DeploymentGuid = deploymentGuid.ToString()
        };
    }
}
