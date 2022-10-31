using Enterspeed.Cli.Api.Identity.Models;
using RestSharp;

namespace Enterspeed.Cli.Services.EnterspeedClient;

public interface IEnterspeedClient
{
    public Task<T> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default);
    public Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default);
    public Task<AuthenticationResult> Authenticate(AuthenticationRequest request, string redirectUrl = null);
    
}