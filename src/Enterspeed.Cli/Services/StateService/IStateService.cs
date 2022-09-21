using Enterspeed.Cli.Api.Identity.Models;
using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Services.StateService;

public interface IStateService
{
    Token Token { get; }
    IdentityUser User { get; }
    void SaveState(Token token, IdentityUser user);
    void SetActiveTenant(TenantId tenantId);
    TenantId ActiveTenant();
}