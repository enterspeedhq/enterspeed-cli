using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class PartialMappingStrategyId : Id
{
    public PartialMappingStrategyId(string idValue)
        : base(idValue)
    {
    }

    public string PartialMappingSchemaGuid { get; set; }

    public int Version { get; set; }

    public static string From(string partialMappingSchemaGuid, int partialMappingSchemaVersion) =>
        $"{IdBase}PartialMappingSchema/{partialMappingSchemaGuid}/Version/{partialMappingSchemaVersion}/Strategy";

    public static bool TryParse(string partialMappingStrategyId, out PartialMappingStrategyId result)
    {
        try
        {
            result = Parse(partialMappingStrategyId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static PartialMappingStrategyId Parse(string partialMappingStrategyId)
    {
        CheckIdValidity(partialMappingStrategyId);
        if (!partialMappingStrategyId.EndsWith("Strategy", StringComparison.Ordinal))
        {
            throw new InvalidIdFormatException("Strategy");
        }

        var gidValues = GetIdValues(partialMappingStrategyId);
        ValidateOrder(gidValues, "PartialMappingSchema", "Version");

        var partialMappingSchemaGuidAsString = gidValues.FirstOrDefault(x => x.Key == "PartialMappingSchema").Value;
        var partialMappingSchemaGuid = GetValidatedGuid(partialMappingSchemaGuidAsString);

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        return new PartialMappingStrategyId(
            From(
                partialMappingSchemaGuid.ToString(),
                version))
        {
            PartialMappingSchemaGuid = partialMappingSchemaGuid.ToString(),
            Version = version
        };
    }
}
