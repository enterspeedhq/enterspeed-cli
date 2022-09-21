namespace Enterspeed.Cli.Api.Identity.Models;

public class AuthenticationCallback
{
    public string Code { get; set; }
    public string State { get; set; }
}