using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.Identity.Models;

public class AuthenticationResult
{
    public bool IsValid { get; set; }
    public string RedirectUrl { get; set; }

    [JsonPropertyName("links")]
    public MetaLinks Links { get; set; }

    public Token Token { get; set; }
    public IdentityUser User { get; set; }
}