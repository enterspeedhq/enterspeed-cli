namespace Enterspeed.Cli.Api.Identity.Models;

public class IdentityUser
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public ICollection<string> Roles { get; set; }

    public Dictionary<string, string[]> Tenants { get; set; }

}