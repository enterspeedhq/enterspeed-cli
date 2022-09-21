using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class ViewId : Id
{
    public ViewId(string idValue)
        : base(idValue)
    {
    }

    public string EnvironmentGuid { get; set; }
    public string SourceGuid { get; set; }
    public string OriginId { get; set; }
    public string ViewHandle { get; set; }

    public static string Create(
        string environmentGuid,
        string sourceGuid,
        string originId,
        string viewHandle) => From(environmentGuid, sourceGuid, originId, viewHandle);

    public static string From(
        string environmentGuid,
        string sourceGuid,
        string originId,
        string viewHandle) => $"{IdBase}Environment/{environmentGuid}/Source/{sourceGuid}/Entity/{originId}/View/{viewHandle}";

    public static bool TryParse(string viewId, out ViewId result)
    {
        try
        {
            result = Parse(viewId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static ViewId Parse(string viewId)
    {
        CheckIdValidity(viewId);

        var environment = EnvironmentId.Parse(viewId);
        var sourceEntity = SourceEntityId.Parse(viewId);

        var gidValues = GetIdValues(viewId);

        if (!gidValues.ContainsKey("View"))
        {
            throw new InvalidIdFormatException("View");
        }

        var viewHandle = gidValues["View"];

        ValidateOrder(gidValues, "Environment", "Source", "Entity", "View");

        return new ViewId(From(environment.EnvironmentGuid, sourceEntity.SourceGuid, sourceEntity.OriginId, viewHandle))
        {
            EnvironmentGuid = environment.EnvironmentGuid,
            SourceGuid = sourceEntity.SourceGuid,
            OriginId = sourceEntity.OriginId,
            ViewHandle = viewHandle
        };
    }
}