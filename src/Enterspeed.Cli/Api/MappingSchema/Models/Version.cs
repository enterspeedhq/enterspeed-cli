using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Api.MappingSchema.Models;

public class Version
{
    public MappingSchemaId Id { get; set; }
    public string Data { get; set; }
    public string Format { get; set; }
    public bool IsEditAble { get; set; }
}