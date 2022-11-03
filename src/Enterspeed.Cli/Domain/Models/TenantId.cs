using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class TenantId : Id
{
    public TenantId(string idValue)
        : base(idValue)
    {
    }

    public string TenantGuid { get; set; }

    public static string Create() => From(Guid.NewGuid().ToString());

    public static string From(string tenantGuid) => $"{IdBase}Tenant/{tenantGuid}";

    public static bool TryParse(string tenantId, out TenantId result)
    {
        try
        {
            result = Parse(tenantId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static TenantId Parse(string tenantId)
    {
        CheckIdValidity(tenantId);
        var gidValues = GetIdValues(tenantId);

        if (!gidValues.ContainsKey("Tenant"))
        {
            throw new InvalidIdFormatException("Tenant");
        }

        ValidateOrder(gidValues, "Tenant");

        var guidAsString = gidValues.FirstOrDefault(x => x.Key == "Tenant").Value;

        var guid = GetValidatedGuid(guidAsString);

        return new TenantId(From(guid.ToString()))
        {
            TenantGuid = guid.ToString()
        };
    }
}