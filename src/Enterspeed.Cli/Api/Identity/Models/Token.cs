namespace Enterspeed.Cli.Api.Identity.Models;

public class Token
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public double ExpiresIn { get; set; }

    public string Scope { get; set; }

    public string TokenType { get; set; }
}