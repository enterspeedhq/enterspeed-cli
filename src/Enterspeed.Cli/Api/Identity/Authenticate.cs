using System.Net;
using Enterspeed.Cli.Api.Identity.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;

namespace Enterspeed.Cli.Api.Identity;

public class IdentityRequest : IRequest<AuthenticationResult>
{
}

public class AuthenticationRequestHandler : IRequestHandler<IdentityRequest, AuthenticationResult>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public AuthenticationRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<AuthenticationResult> Handle(IdentityRequest request, CancellationToken cancellationToken)
    {
        var redirectUrl = await GetAuthenticationUrl();
        var authCallback = StartBrowserProcessAndListenForCallback(redirectUrl);

        var response = await _enterspeedClient.Authenticate(
            new AuthenticationRequest
            {
                Type = "authorization_code",
                State = authCallback.State,
                Token = authCallback.Code
            });

        return await Task.FromResult(response);
    }

    private const int CallbackListeningPort = 8081;

    private async Task<string> GetAuthenticationUrl()
    {
        var redirectUrl = $"http://localhost:{CallbackListeningPort}";
        var response = await _enterspeedClient.Authenticate(
            new AuthenticationRequest
            {
                Type = "authorization_code",
                FrontendRedirectUrl = redirectUrl
            }, redirectUrl);


        if (!response.IsValid)
        {
            return response.Links.SignInUrl;
        }

        return null;
    }

    private AuthenticationCallback StartBrowserProcessAndListenForCallback(string redirectUrl)
    {
        string state = null;
        string code = null;

        var task = Task.Run(
            () =>
            {
                using (var listener = new HttpListener())
                {
                    listener.Prefixes.Add($"http://localhost:{CallbackListeningPort}/");
                    listener.Start();
                    while (string.IsNullOrEmpty(state) && string.IsNullOrEmpty(code))
                    {
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        state = request.QueryString.Get("state");
                        code = request.QueryString.Get("code");

                        // Obtain a response object.
                        HttpListenerResponse response = context.Response;
                        // Construct a response.
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("You can close this window!");
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                        listener.Stop();

                        return Task.CompletedTask;
                    }
                }

                return Task.CompletedTask;
            });

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = redirectUrl
        };
        System.Diagnostics.Process.Start(psi);

        Task.WaitAll(task);
        return new AuthenticationCallback()
        {
            Code = code,
            State = state
        };
    }
}