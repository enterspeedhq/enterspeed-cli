using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.StateService;
using System.CommandLine.Invocation;
using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Tenant;

public class SetActiveTenantCommand : Command
{
    public SetActiveTenantCommand() : base(name: "set", "Set the active tenant")
    {
        AddArgument(new Argument<string>("id", "Id of the tenant") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IStateService _stateService;
        private readonly ILogger<SetActiveTenantCommand> _logger;

        public Handler(IStateService stateService, ILogger<SetActiveTenantCommand> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        public string Id { get; set; }

        public Task<int> InvokeAsync(InvocationContext context)
        {   
            var tenantIds = _stateService.User?.Tenants?.Keys;

            if (tenantIds == null || !tenantIds.Any())
            {
                _logger.LogError("Could not find tenant");
                return Task.FromResult(1);
            }
            var tenants = tenantIds.Select(TenantId.Parse);

            var activeTenant = tenants.FirstOrDefault(x => x.IdValue == Id);
            if (activeTenant == null)
            {
                _logger.LogError("Could not find tenant");
                return Task.FromResult(1);
            }

            _logger.LogInformation($"Setting active tenant to: {activeTenant.IdValue}");
            _stateService.SetActiveTenant(activeTenant);

            return Task.FromResult(0);
        }
    }
}