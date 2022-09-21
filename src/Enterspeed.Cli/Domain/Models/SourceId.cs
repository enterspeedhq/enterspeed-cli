using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class SourceId : Id
{
    public SourceId(string idValue)
        : base(idValue)
    {
    }

    public string SourceGuid { get; set; }

    public static SourceId Default() => Parse(From(Guid.Empty.ToString()));

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string sourceGuid) => $"{IdBase}Source/{sourceGuid}";

    public static bool TryParse(string sourceId, out SourceId result)
    {
        try
        {
            result = Parse(sourceId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static SourceId Parse(string sourceId)
    {
        CheckIdValidity(sourceId);

        var gidValues = GetIdValues(sourceId);

        if (!gidValues.ContainsKey("Source"))
        {
            throw new InvalidIdFormatException("Source");
        }

        ValidateOrder(gidValues, "Source");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "Source").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new SourceId(From(guid.ToString()))
        {
            SourceGuid = guid.ToString()
        };
    }
}