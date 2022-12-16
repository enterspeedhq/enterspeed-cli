using Enterspeed.Cli.Services.StateService;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Configuration;
using Enterspeed.Cli.Services.ConsoleOutput;

namespace Enterspeed.Cli.Commands.Tenant;
public class CurrentTenantCommand : Command
{
    public CurrentTenantCommand() : base(name: "current", "Get the active tenant")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IStateService _stateService;
        private readonly IOutputService _outputService;
        private readonly GlobalOptions _globalOptions;

        public Handler(IStateService stateService, IOutputService outputService, GlobalOptions globalOptions)
        {
            _stateService = stateService;
            _outputService = outputService;
            _globalOptions = globalOptions;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            if (!string.IsNullOrEmpty(_globalOptions.ApiKeyValue))
            {
                _outputService.Write("Using tenant from api key");
                return Task.FromResult(0);
            }

            var tenantId = _stateService.ActiveTenant();
            if (tenantId == null)
            {
                _outputService.Write("No active tenant found");
                return Task.FromResult(1);
            }

            _outputService.Write(tenantId);

            return Task.FromResult(0);
        }
    }
}