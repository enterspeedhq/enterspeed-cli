using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class HostnameId : Id
{
    public HostnameId(string idValue)
        : base(idValue)
    {
    }

    public string TenantGuid { get; private set; }
    public string Hostname { get; private set; }

    public static string Create(string tenantGuid, string hostname) => From(tenantGuid, hostname);

    public static string From(string tenantGuid, string hostname) => $"{IdBase}Tenant/{tenantGuid}/Hostname/{hostname}";

    public static HostnameId Parse(string hostnameId)
    {
        CheckIdValidity(hostnameId);

        var gidValues = GetIdValues(hostnameId);

        if (!gidValues.ContainsKey("Hostname"))
        {
            throw new InvalidIdFormatException("Hostname");
        }

        ValidateOrder(gidValues, "Tenant", "Hostname");

        var hostname = gidValues.FirstOrDefault(x => x.Key == "Hostname").Value;

        var tenant = TenantId.Parse(hostnameId);

        return new HostnameId(From(tenant.TenantGuid, hostname))
        {
            TenantGuid = tenant.TenantGuid,
            Hostname = hostname
        };
    }
}