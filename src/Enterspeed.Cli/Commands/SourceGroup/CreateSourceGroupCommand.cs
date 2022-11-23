using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.SourceGroup;

namespace Enterspeed.Cli.Commands.SourceGroup;

public class CreateSourceGroupCommand : Command
{
    public CreateSourceGroupCommand() : base(name: "create", "Create source group")
    {
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of source group") { IsRequired = true });
        AddOption(new Option<string>(new[] { "--alias", "-a" }, "Alias of source group") { IsRequired = true });
        AddOption(new Option<string>(new[] { "--type", "-t" }, "Source group type"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var request = new CreateSourceGroupRequest
            {
                Name = Name,
                Alias = Alias,
                Type = Type
            };

            var domain = await _mediator.Send(request);

            _outputService.Write(domain);

            return 0;
        }
    }

}