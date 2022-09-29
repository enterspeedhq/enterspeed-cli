﻿using System.Net;
using Enterspeed.Cli.Api.Identity.Models;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Services.StateService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Enterspeed.Cli.Services.EnterspeedClient;

public class EnterspeedClient : IEnterspeedClient, IDisposable
{
    private readonly ILogger<EnterspeedClient> _logger;
    private readonly IStateService _stateService;
    private readonly RestClient _client;
    private readonly string _apiKey;

    public EnterspeedClient(ILogger<EnterspeedClient> logger, IConfiguration configuration, IStateService stateService)
    {
        _logger = logger;
        _stateService = stateService;
        _apiKey = ApiKey.Get();

        var settings = configuration.GetRequiredSection("Settings").Get<Settings>();
        var options = new RestClientOptions(settings.EnterspeedApiUri);

        _client = new RestClient(options)
        {
        };
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
            await HandleUnauthorized<T>(request, cancellationToken);
        }

        if (!response.IsSuccessful)
        {
            _logger.LogError($"Unsuccessful: {response.StatusCode}");
        }

        return response.Data;
    }

    private void AddHeaders(RestRequest request)
    {
        var tenantId = _stateService.ActiveTenant();
        _logger.LogInformation($"TenantId: {tenantId.IdValue}");

        request.AddHeader("X-Tenant-Id", tenantId.IdValue);

        if (!string.IsNullOrEmpty(_apiKey))
        {
            request.AddHeader("X-Api-Key", _apiKey);
        }
        else
        {
            request.AddHeader("Authorization", $"Bearer {_stateService.Token.AccessToken}");
        }
    }

    private async Task HandleUnauthorized<T>(RestRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_apiKey))
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
                var response = await _client.ExecuteAsync<T>(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Unauthorized! You need to run 'es-cli login' again");
                }
            }
        }
    }

    private bool IsAuthenticationSetUp()
    {
        if (!string.IsNullOrEmpty(_apiKey))
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