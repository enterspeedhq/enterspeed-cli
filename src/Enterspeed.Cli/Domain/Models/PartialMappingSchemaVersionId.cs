using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class PartialMappingSchemaVersionId : Id
{
    public PartialMappingSchemaVersionId(string idValue)
        : base(idValue)
    {
    }

    public string PartialMappingSchemaGuid { get; set; }

    public int Version { get; set; }

    public static string From(string partialMappingSchemaId, int version) => $"{IdBase}PartialMappingSchema/{partialMappingSchemaId}/Version/{version}";

    public static bool TryParse(string partialMappingSchemaId, out PartialMappingSchemaVersionId result)
    {
        try
        {
            result = Parse(partialMappingSchemaId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static PartialMappingSchemaVersionId Parse(string partialMappingSchemaId)
    {
        CheckIdValidity(partialMappingSchemaId);
        var gidValues = GetIdValues(partialMappingSchemaId);
        if (!gidValues.ContainsKey("Version"))
        {
            throw new InvalidIdFormatException("Version");
        }

        var partialMappingSchemaGid = PartialMappingSchemaId.Parse(partialMappingSchemaId);
        ValidateOrder(gidValues, "PartialMappingSchema", "Version");

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        return new PartialMappingSchemaVersionId(From(partialMappingSchemaGid.PartialMappingSchemaGuid, version))
        {
            PartialMappingSchemaGuid = partialMappingSchemaGid.PartialMappingSchemaGuid,
            Version = version
        };
    }
}
