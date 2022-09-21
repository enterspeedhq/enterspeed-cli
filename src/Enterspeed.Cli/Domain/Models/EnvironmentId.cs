using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class EnvironmentId : Id
{
    public EnvironmentId(string idValue)
        : base(idValue)
    {
    }

    public string EnvironmentGuid { get; set; }

    public static EnvironmentId Default() => Parse(From(Guid.Empty.ToString()));

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string environmentGuid) => $"{IdBase}Environment/{environmentGuid}";

    public static bool TryParse(string environmentId, out EnvironmentId result)
    {
        try
        {
            result = Parse(environmentId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static EnvironmentId Parse(string environmentId)
    {
        CheckIdValidity(environmentId);

        var gidValues = GetIdValues(environmentId);

        if (!gidValues.ContainsKey("Environment"))
        {
            throw new InvalidIdFormatException("Environment");
        }

        ValidateOrder(gidValues, "Environment");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "Environment").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new EnvironmentId(From(guid.ToString()))
        {
            EnvironmentGuid = guid.ToString()
        };
    }
}