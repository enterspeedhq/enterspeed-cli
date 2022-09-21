using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class EnvironmentClientId : Id
{
    public EnvironmentClientId(string idValue)
        : base(idValue)
    {
    }

    public string EnvironmentGuid { get; set; }
    public string ClientGuid { get; set; }

    public static string Create(string environmentGuid) => From(environmentGuid, Guid.NewGuid().ToString());

    public static string From(string environmentGuid, string clientGuid) => $"{IdBase}Environment/{environmentGuid}/Client/{clientGuid}";

    public static bool TryParse(string environmentClientId, out EnvironmentClientId result)
    {
        try
        {
            result = Parse(environmentClientId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static EnvironmentClientId Parse(string environmentClientId)
    {
        CheckIdValidity(environmentClientId);
        var gidValues = GetIdValues(environmentClientId);

        if (!gidValues.ContainsKey("Client"))
        {
            throw new InvalidIdFormatException("Client");
        }

        var environment = EnvironmentId.Parse(environmentClientId);

        ValidateOrder(gidValues, "Environment", "Client");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "Client").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new EnvironmentClientId(From(environment.EnvironmentGuid, guid.ToString()))
        {
            EnvironmentGuid = environment.EnvironmentGuid,
            ClientGuid = guid.ToString()
        };
    }
}