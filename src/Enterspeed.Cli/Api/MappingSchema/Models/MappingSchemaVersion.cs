using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Api.MappingSchema.Models;

public class MappingSchemaVersion
{
    public MappingSchemaVersionId Id { get; set; }
    public string Data { get; set; }
    public string Format { get; set; }
    public bool IsEditAble { get; set; }
}