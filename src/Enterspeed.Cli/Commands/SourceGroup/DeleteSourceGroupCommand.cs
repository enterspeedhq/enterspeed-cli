using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.SourceGroup;

namespace Enterspeed.Cli.Commands.SourceGroup;

public class DeleteSourceGroupCommand : ConfirmCommand
{
    public DeleteSourceGroupCommand() : base(name: "delete", "Delete source group")
    {
        AddArgument(new Argument<Guid>("id", "Id of the source group") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Guid Id { get; set; }


        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!Yes && !GetConfirmation())
            {
                return 0;
            }

            var response = await _mediator.Send(new DeleteSourceGroupRequest(SourceGroupId.Parse(SourceGroupId.From(Id.ToString()))));
            _outputService.Write(response);

            return 0;
        }
    }
}