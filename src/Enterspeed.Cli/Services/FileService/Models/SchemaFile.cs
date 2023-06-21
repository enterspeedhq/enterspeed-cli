using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Services.FileService.Models;

public class SchemaFile
{
    public string Alias { get; }
    public SchemaType SchemaType { get; }
    public SchemaBaseProperties SchemaBaseProperties { get; }

    public SchemaFile(string alias, SchemaType schemaType, SchemaBaseProperties schemaBaseProperties)
    {
        Alias = alias;
        SchemaType = schemaType;
        SchemaBaseProperties = schemaBaseProperties;
    }
}