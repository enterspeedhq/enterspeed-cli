using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class MappingSchemaId : Id
{
    public MappingSchemaId(string idValue)
        : base(idValue)
    {
    }

    public string MappingSchemaGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string mappingSchemaGuid) => $"{IdBase}MappingSchema/{mappingSchemaGuid}";

    public static bool TryParse(string mappingSchemaId, out MappingSchemaId result)
    {
        try
        {
            result = Parse(mappingSchemaId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static MappingSchemaId Parse(string mappingSchemaId)
    {
        CheckIdValidity(mappingSchemaId);
        var gidValues = GetIdValues(mappingSchemaId);

        if (!gidValues.ContainsKey("MappingSchema"))
        {
            throw new InvalidIdFormatException("MappingSchema");
        }

        ValidateOrder(gidValues, "MappingSchema");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "MappingSchema").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new MappingSchemaId(From(guid.ToString()))
        {
            MappingSchemaGuid = guid.ToString()
        };
    }
}