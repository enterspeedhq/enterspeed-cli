namespace Enterspeed.Cli.Services.FileService.Models;

public class SchemaFile
{
    public string Alias { get; }
    public SchemaBaseProperties SchemaBaseProperties { get; }

    public SchemaFile(string alias, SchemaBaseProperties schemaBaseProperties)
    {
        Alias = alias;
        SchemaBaseProperties = schemaBaseProperties;
    }
}