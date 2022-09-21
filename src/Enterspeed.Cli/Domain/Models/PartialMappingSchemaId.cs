using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class PartialMappingSchemaId : Id
{
    public PartialMappingSchemaId(string idValue)
        : base(idValue)
    {
    }

    public string PartialMappingSchemaGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string partialMappingSchemaGuid) => $"{IdBase}PartialMappingSchema/{partialMappingSchemaGuid}";

    public static bool TryParse(string partialMappingSchemaId, out PartialMappingSchemaId result)
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

    public static PartialMappingSchemaId Parse(string partialMappingSchemaId)
    {
        CheckIdValidity(partialMappingSchemaId);
        var gidValues = GetIdValues(partialMappingSchemaId);

        if (!gidValues.ContainsKey("PartialMappingSchema"))
        {
            throw new InvalidIdFormatException("PartialMappingSchema");
        }

        ValidateOrder(gidValues, "PartialMappingSchema");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "PartialMappingSchema").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new PartialMappingSchemaId(From(guid.ToString()))
        {
            PartialMappingSchemaGuid = guid.ToString()
        };
    }
}
