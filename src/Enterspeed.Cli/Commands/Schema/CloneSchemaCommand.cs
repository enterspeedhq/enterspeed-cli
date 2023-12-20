using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Enterspeed.Cli.Commands.Schema;

internal class CloneSchemaCommand : Command
{
    public CloneSchemaCommand() : base("clone", "Creates all schemas on tenant as json files on disk, in latest versions under /schemas")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ISchemaFileService _schemaFileService;

        public Handler(IMediator mediator, IOutputService outputService, ISchemaFileService schemaFileService)
        {
            _mediator = mediator;
            _outputService = outputService;
            _schemaFileService = schemaFileService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var allSchemas = await _mediator.Send(new QueryMappingSchemasRequest());
            var schemaResponses = new List<GetMappingSchemaResponse>();

            foreach (var schema in allSchemas)
            {
                var schemaResponse = await _mediator.Send(new GetMappingSchemaRequest
                {
                    MappingSchemaId = schema.Id.MappingSchemaGuid
                });

                if (schemaResponse != null)
                {
                    schemaResponses.Add(schemaResponse);
                }
            }

            foreach (var schema in schemaResponses)
            {
                _schemaFileService.CreateSchema(schema.ViewHandle, schema.Type, schema.Version, schema.Name);
            }

            _outputService.Write("Successfully cloned all schemas");

            return 0;
        }
    }
}