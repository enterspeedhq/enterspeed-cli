using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class MappingSchemaVersionId : Id
{
    public MappingSchemaVersionId(string idValue)
        : base(idValue)
    {
    }

    public string MappingSchemaGuid { get; set; }

    public int Version { get; set; }

    public static string From(string mappingSchemaGuid, int version) => $"{IdBase}MappingSchema/{mappingSchemaGuid}/Version/{version}";

    public static bool TryParse(string mappingSchemaVersionId, out MappingSchemaVersionId result)
    {
        try
        {
            result = Parse(mappingSchemaVersionId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static MappingSchemaVersionId Parse(string mappingSchemaVersionId)
    {
        CheckIdValidity(mappingSchemaVersionId);
        var gidValues = GetIdValues(mappingSchemaVersionId);
        if (!gidValues.ContainsKey("Version"))
        {
            throw new InvalidIdFormatException("Version");
        }

        var mappingSchemaGid = MappingSchemaId.Parse(mappingSchemaVersionId);
        ValidateOrder(gidValues, "MappingSchema", "Version");

        var intAsString = gidValues.FirstOrDefault(x => x.Key == "Version").Value;
        if (!int.TryParse(intAsString, out var version))
        {
            throw new InvalidIdFormatException("Version");
        }

        return new MappingSchemaVersionId(From(mappingSchemaGid.MappingSchemaGuid, version))
        {
            MappingSchemaGuid = mappingSchemaGid.MappingSchemaGuid,
            Version = version
        };
    }
}
