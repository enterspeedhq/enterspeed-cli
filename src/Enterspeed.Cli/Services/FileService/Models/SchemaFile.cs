using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Services.FileService.Models;

public class SchemaFile
{
    public string Alias { get; }
    public SchemaType SchemaType { get; }
    public object Content { get; }
    public string Format { get; }
    public string RelativeDirectoryPath { get; }

    public SchemaFile(string alias, SchemaType schemaType, object content, string format, string relativeDirectoryPath)
    {
        Alias = alias;
        SchemaType = schemaType;
        Content = content;
        Format = format;
        RelativeDirectoryPath = relativeDirectoryPath;
    }
}