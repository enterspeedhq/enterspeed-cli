using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.SourceGroup;

namespace Enterspeed.Cli.Commands.SourceGroup;

public class UpdateSourceGroupCommand : Command
{
    public UpdateSourceGroupCommand() : base(name: "update", "Update source group")
    {
        AddArgument(new Argument<Guid>("id", "Id of the source group") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of source group"));
        AddOption(new Option<string>(new[] { "--type", "-t" }, "Source group type"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }


        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var request = new UpdateSourceGroupRequest(SourceGroupId.Parse(SourceGroupId.From(Id.ToString())));
            if (!string.IsNullOrEmpty(Name))
            {
                request.Name = Name;
            }

            if (!string.IsNullOrEmpty(Type))
            {
                request.Type = Type;
            }

            var response = await _mediator.Send(request);

            _outputService.Write(response);
            return 0;
        }
    }
}