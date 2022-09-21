using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.Identity.Models;

public class AuthenticationRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("frontendRedirectUrl")]
    public string FrontendRedirectUrl { get; set; }
}