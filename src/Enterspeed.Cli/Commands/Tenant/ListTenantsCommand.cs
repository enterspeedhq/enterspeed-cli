using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Tenant;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.StateService;

namespace Enterspeed.Cli.Commands.Tenant;

public  class ListTenantsCommand : Command
{
    public ListTenantsCommand() : base(name: "list", "List tenants")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly IStateService _stateService;

        public Handler(IMediator mediator, IOutputService outputService, IStateService stateService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _stateService = stateService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var tenantIds = _stateService.User?.Tenants?.Keys;

            if (tenantIds == null || !tenantIds.Any()) return 0;
            var tenantGuids = tenantIds.Select(TenantId.Parse).Select(x => x.TenantGuid).ToArray();

            var tenants = await _mediator.Send(new GetTenantsRequest {TenantIds = tenantGuids });

            _outputService.Write(tenants, Output);
            return 0;
        }
    }
}