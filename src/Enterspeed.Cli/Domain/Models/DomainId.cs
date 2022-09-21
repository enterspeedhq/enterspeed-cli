using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class DomainId : Id
{
    public DomainId(string idValue)
        : base(idValue)
    {
    }

    public Guid DomainGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string domainGuid) => $"{IdBase}Domain/{domainGuid}";

    public static DomainId Parse(string domainId)
    {
        CheckIdValidity(domainId);

        var gidValues = GetIdValues(domainId);

        if (!gidValues.ContainsKey("Domain"))
        {
            throw new InvalidIdFormatException("Domain");
        }

        ValidateOrder(gidValues, "Domain");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "Domain").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new DomainId(From(guid.ToString()))
        {
            DomainGuid = guid
        };
    }
}