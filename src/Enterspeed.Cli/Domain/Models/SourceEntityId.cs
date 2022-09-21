using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class SourceEntityId : Id
{
    public SourceEntityId(string idValue)
        : base(idValue)
    {
    }

    public string SourceGuid { get; set; }
    public string OriginId { get; set; }

    public static string Create(string sourceGuid, string originId) => From(sourceGuid, originId);

    public static string From(string sourceGuid, string originId) => $"{IdBase}Source/{sourceGuid}/Entity/{originId}";

    public static bool TryParse(string sourceEntityId, out SourceEntityId result)
    {
        try
        {
            result = Parse(sourceEntityId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static SourceEntityId Parse(string sourceEntityId)
    {
        CheckIdValidity(sourceEntityId);

        var source = SourceId.Parse(sourceEntityId);

        var gidValues = GetIdValues(sourceEntityId);

        if (!gidValues.ContainsKey("Entity"))
        {
            throw new InvalidIdFormatException("Entity");
        }

        var originId = gidValues["Entity"];

        ValidateOrder(gidValues, "Source", "Entity");

        return new SourceEntityId(From(source.SourceGuid, originId))
        {
            SourceGuid = source.SourceGuid,
            OriginId = originId
        };
    }
}