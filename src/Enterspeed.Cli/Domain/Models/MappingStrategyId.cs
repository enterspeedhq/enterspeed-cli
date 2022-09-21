using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class MappingStrategyId : Id
{
    public MappingStrategyId(string idValue)
        : base(idValue)
    {
    }

    public string MappingSchemaGuid { get; set; }

    public int Version { get; set; }

    public static string From(string mappingSchemaGuid, int mappingSchemaVersion) =>
        $"{IdBase}MappingSchema/{mappingSchemaGuid}/Version/{mappingSchemaVersion}/Strategy";

    public static bool TryParse(string mappingStrategyId, out MappingStrategyId result)
    {
        try
        {
            result = Parse(mappingStrategyId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static MappingStrategyId Parse(string mappingStrategyId)
    {
        CheckIdValidity(mappingStrategyId);
        if (!mappingStrategyId.EndsWith("Strategy", StringComparison.Ordinal))
        {
            throw new InvalidIdFormatException("Strategy");
        }

        var gidValues = GetIdValues(mappingStrategyId);
        ValidateOrder(gidValues, "MappingSchema", "Version", "Environment");

        var mappingSchemaGuidAsString = gidValues.FirstOrDefault(x => x.Key == "MappingSchema").Value;
        var mappingSchemaGuid = GetValidatedGuid(mappingSchemaGuidAsString);

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        return new MappingStrategyId(
            From(
                mappingSchemaGuid.ToString(),
                version))
        {
            MappingSchemaGuid = mappingSchemaGuid.ToString(),
            Version = version
        };
    }
}
