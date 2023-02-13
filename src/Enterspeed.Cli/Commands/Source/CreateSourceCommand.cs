using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Source;
using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Commands.Source;
public class CreateSourceCommand : Command
{
    public CreateSourceCommand() : base(name: "create", "Create source in source group")
    {
        AddArgument(new Argument<Guid>("id", "Id of the source group") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of source group") { IsRequired = true });
        AddOption(new Option<string[]>(new[] { "--environment", "-e" }, "Target environment for deploy") { IsRequired = true });
        AddOption(new Option<string>(new[] { "--type", "-t" }, "Source group type") { IsRequired = true });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] Environment { get; set; }
        public string Type { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var request = new CreateSourceRequest(SourceGroupId.Parse(SourceGroupId.From(Id.ToString())))
            {
                Name = Name,
                Type = Type,
                EnvironmentIds = Environment
            };

            var domain = await _mediator.Send(request);

            _outputService.Write(domain);

            return 0;
        }
    }
}