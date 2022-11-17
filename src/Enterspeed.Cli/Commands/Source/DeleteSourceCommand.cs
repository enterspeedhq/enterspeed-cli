using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Source;

namespace Enterspeed.Cli.Commands.Source;

public class DeleteSourceCommand : ConfirmCommand
{
    public DeleteSourceCommand() : base(name: "delete", "Delete source")
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

            var response = await _mediator.Send(new DeleteSourceRequest(SourceId.Parse(SourceId.From(Id.ToString()))));
            _outputService.Write(response);

            return 0;
        }
    }
}