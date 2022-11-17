using System.Text.Json;
using Enterspeed.Cli.Api.Identity.Models;
using Enterspeed.Cli.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.StateService;

public class StateService : IStateService
{
    private readonly ILogger<StateService> _logger;
    private readonly string _stateFilePath;

    public StateService(ILogger<StateService> logger)
    {
        _logger = logger;

        var userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _stateFilePath = Path.Combine(userFolderPath, ".enterspeed", "cli.state.json");

        LoadState();
    }

    public IdentityUser User { get; private set; }

    public Token Token { get; private set; }

    private TenantId ActiveTenantId { get; set; }

    public void SaveState(Token token, IdentityUser user)
    {
        User = user;
        Token = token;

        if (ActiveTenantId == null || !User.Tenants.Keys.Contains(ActiveTenantId.IdValue))
        {
            ActiveTenantId = TenantId.Parse(User.Tenants.FirstOrDefault().Key);
        }

        var jsonString = JsonSerializer.Serialize(new State
        {
            Token = token,
            User = user,
            ActiveTenantId = ActiveTenantId.IdValue
        });
        Directory.CreateDirectory(Path.GetDirectoryName(_stateFilePath)!);
        File.WriteAllText(_stateFilePath, jsonString);
        _logger.LogInformation("State saved");
    }

    public void SetActiveTenant(TenantId tenantId)
    {
        _logger.LogInformation($"Setting active tenant to: {tenantId.IdValue}");
        ActiveTenantId = tenantId;
        SaveState(Token, User);
    }

    public TenantId ActiveTenant()
    {
        if (ActiveTenantId == null)
        {
            ActiveTenantId = TenantId.Parse(User.Tenants.Keys.FirstOrDefault());
        }
        return ActiveTenantId;
    }

    private void LoadState()
    {
        try
        {
            var jsonString = File.ReadAllText(_stateFilePath);
            var state = JsonSerializer.Deserialize<State>(jsonString);
            if (state != null)
            {
                Token = state.Token;
                User = state.User;
                if (state.ActiveTenantId != null)
                {
                    ActiveTenantId = TenantId.Parse(state.ActiveTenantId);
                }
            }
        }
        catch (Exception)
        {
            _logger.LogWarning("No state file found.");
        }
    }
}

public class State
{
    public Token Token { get; set; }
    public IdentityUser User { get; set; }
    public string ActiveTenantId { get; set; }
}