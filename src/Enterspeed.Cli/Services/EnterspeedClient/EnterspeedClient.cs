using System.Net;
using System.Text.Json;
using Enterspeed.Cli.Api.Identity.Models;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Services.StateService;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Enterspeed.Cli.Services.EnterspeedClient;

public class EnterspeedClient : IEnterspeedClient, IDisposable
{
    private readonly ILogger<EnterspeedClient> _logger;
    private readonly IStateService _stateService;
    private readonly RestClient _client;
    private readonly string _apiKeyValue;

    public EnterspeedClient(ILogger<EnterspeedClient> logger, ISettingsService settingsService, IStateService stateService, GlobalOptions globalOptions)
    {
        _logger = logger;
        _stateService = stateService;
        _apiKeyValue = globalOptions?.ApiKeyValue;

        var customEndpointUri = globalOptions?.CustomEndpoint;

        RestClientOptions restOptions;
        if (customEndpointUri != null)
        {
            restOptions = new RestClientOptions(customEndpointUri);
        }
        else
        {
            var settings = settingsService.GetSettings();
            restOptions = new RestClientOptions(settings.EnterspeedApiUri);
        }

        _client = new RestClient(restOptions);
    }

    public async Task<T> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticationSetUp())
        {
            return default;
        }

        AddHeaders(request);

        var response = await _client.ExecuteAsync<T>(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response = await HandleUnauthorized<T>(request, cancellationToken);
        }

        if (!response.IsSuccessful)
        {
            _logger.LogError($"Unsuccessful: {response.StatusCode}");
            _logger.LogWarning(JsonSerializer.Serialize(response.Data));
        }

        //_logger.LogInformation(JsonSerializer.Serialize(response.Data));

        return response.Data;
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticationSetUp())
        {
            return default;
        }

        AddHeaders(request);

        var response = await _client.ExecuteAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorized(request, cancellationToken);
        }

        return response;
    }

    private void AddHeaders(RestRequest request)
    {
        if (!string.IsNullOrEmpty(_apiKeyValue))
        {
            request.AddHeader("X-Api-Key", _apiKeyValue);
        }
        else
        {
            var tenantId = _stateService.ActiveTenant();
            _logger.LogInformation($"TenantId: {tenantId.IdValue}");

            request.AddHeader("X-Tenant-Id", tenantId.IdValue);
            request.AddHeader("Authorization", $"Bearer {_stateService.Token.AccessToken}");
        }
    }

    private async Task<RestResponse<T>> HandleUnauthorized<T>(RestRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_apiKeyValue))
        {
            _logger.LogError("Unauthorized, make sure that you are using the correct api key");
            throw new Exception("Unauthorized");
        }
        else
        {
            // If Unauthorized, refresh token and try again
            _logger.LogInformation("Unauthorized, trying to refresh token");
            var refreshResult = await RefreshToken(_stateService.Token.RefreshToken);

            if (refreshResult.IsValid)
            {
                request.AddOrUpdateHeader("Authorization", $"Bearer {_stateService.Token.AccessToken}");
                var response = await _client.ExecuteAsync<T>(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Unauthorized! You need to run 'es-cli login' again");
                }

                return response;
            }

            _logger.LogError("Unauthorized, refresh token failed");
            throw new Exception("Unauthorized");
        }
    }

    private async Task HandleUnauthorized(RestRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_apiKeyValue))
        {
            _logger.LogError("Unauthorized, make sure that you are using the correct api key");
        }
        else
        {
            // If Unauthorized, refresh token and try again
            _logger.LogInformation("Unauthorized, trying to refresh token");
            var refreshResult = await RefreshToken(_stateService.Token.RefreshToken);

            if (refreshResult.IsValid)
            {
                request.AddOrUpdateHeader("Authorization", $"Bearer {_stateService.Token.AccessToken}");
                var response = await _client.ExecuteAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Unauthorized! You need to run 'es-cli login' again");
                }
            }
        }
    }

    private bool IsAuthenticationSetUp()
    {
        if (!string.IsNullOrEmpty(_apiKeyValue))
        {
            return true;
        }

        if (_stateService.Token == null)
        {
            _logger.LogError("No token found. Please run 'es-cli login' first");
            return false;
        }

        return true;
    }

    public async Task<AuthenticationResult> Authenticate(AuthenticationRequest request, string redirectUrl = null)
    {
        var authRequest = new RestRequest("identity/authenticate", Method.Post)
            .AddJsonBody(request);

        if (redirectUrl != null)
        {
            authRequest.AddQueryParameter("redirect_uri", redirectUrl);
        }

        var response = await _client.ExecuteAsync<AuthenticationResult>(authRequest);

        if (response.IsSuccessful && response.Data != null)
        {
            _stateService.SaveState(response.Data.Token, response.Data.User);
        }

        return response.Data;
    }

    private async Task<AuthenticationResult> RefreshToken(string refreshToken)
    {
        return await Authenticate(
            new AuthenticationRequest
            {
                Type = "refresh_token",
                Token = refreshToken
            });
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}