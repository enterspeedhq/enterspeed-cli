using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class SourceGroupId : Id
{
    public SourceGroupId(string idValue)
        : base(idValue)
    {
    }

    public string SourceGroupGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string sourceGroupGuid) => $"{IdBase}SourceGroup/{sourceGroupGuid}";

    public static bool TryParse(string sourceGroupId, out SourceGroupId result)
    {
        try
        {
            result = Parse(sourceGroupId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static SourceGroupId Parse(string sourceGroupId)
    {
        CheckIdValidity(sourceGroupId);

        var gidValues = GetIdValues(sourceGroupId);

        if (!gidValues.ContainsKey("SourceGroup"))
        {
            throw new InvalidIdFormatException("SourceGroup");
        }

        ValidateOrder(gidValues, "SourceGroup");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "SourceGroup").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new SourceGroupId(From(guid.ToString()))
        {
            SourceGroupGuid = guid.ToString()
        };
    }
}