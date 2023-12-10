using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Services.FileService.Models;

public class SchemaFile
{
    public string Alias { get; }
    public SchemaType SchemaType { get; }
    public object Content { get; }
    public string Format { get; }
    public string RelativeSchemaDirectory { get; }

    public SchemaFile(string alias, SchemaType schemaType, object content, string format, string relativeSchemaDirectory)
    {
        Alias = alias;
        SchemaType = schemaType;
        Content = content;
        Format = format;
        RelativeSchemaDirectory = relativeSchemaDirectory;
    }
}