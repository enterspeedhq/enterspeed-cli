using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class UserId : Id
{
    public UserId(string idValue)
        : base(idValue)
    {
    }

    public string UserGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string userGuid) => $"{IdBase}User/{userGuid}";

    public static bool TryParse(string userId, out UserId result)
    {
        try
        {
            result = Parse(userId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static UserId Parse(string userId)
    {
        CheckIdValidity(userId);
        var gidValues = GetIdValues(userId);

        if (!gidValues.ContainsKey("User"))
        {
            throw new InvalidIdFormatException("User");
        }

        ValidateOrder(gidValues, "User");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "User").Value;

        var guid = GetValidatedGuid(guidAsString).ToString();

        return new UserId(From(guid))
        {
            UserGuid = guid
        };
    }
}
