using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class RouteHandleId : Id
{
    public RouteHandleId(string idValue)
        : base(idValue)
    {
    }

    public string EnvironmentGuid { get; set; }
    public string Handle { get; set; }

    public static string From(string environmentGuid, string handle)
    {
        if (string.IsNullOrWhiteSpace(handle))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(environmentGuid))
        {
            throw new InvalidIdFormatException("Missing environment id");
        }

        return $"{IdBase}Environment/{environmentGuid}/Route/Handle/{handle}";
    }

    public static bool TryParse(string routeId, out RouteHandleId result)
    {
        try
        {
            result = Parse(routeId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static RouteHandleId Parse(string routeId)
    {
        CheckIdValidity(routeId);

        var environment = EnvironmentId.Parse(routeId);

        var handles = routeId.Split($"{environment.IdValue}/Route/Handle/", StringSplitOptions.RemoveEmptyEntries);
        var handle = handles.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(handle))
        {
            throw new InvalidIdFormatException("Missing handle");
        }

        return new RouteHandleId(From(environment.EnvironmentGuid, handle))
        {
            EnvironmentGuid = environment.EnvironmentGuid,
            Handle = handle
        };
    }
}
