using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.SchemaService;

public class SchemaNameService : ISchemaNameService
{
    private const string SchemaDirectory = "schemas";

    public bool IsDirectorySchemaName(string schemaName)
    {
        return schemaName.Contains('/');
    }

    public string BuildNewSchemaName(string existingSchemaName, string relativeDirectoryPathOnDisk)
    {
        var lastSegment = Path.GetFileName(existingSchemaName.TrimEnd('/'));

        if (!string.IsNullOrEmpty(relativeDirectoryPathOnDisk))
        {
            return $"{relativeDirectoryPathOnDisk.Replace("\\", "/")}/{lastSegment}";
        }

        return lastSegment;
    }

    public string GetSchemaName(SchemaFile schemaFile)
    {
        if (!string.IsNullOrEmpty(schemaFile.RelativeSchemaDirectory))
        {
            return $"{schemaFile.RelativeSchemaDirectory.Replace("\\", "/")}/{schemaFile.Alias}";
        }

        return schemaFile.Alias;
    }

    public string GetAliasFromFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath)
            .Replace(".partial", "")
            .Replace(".full", "")
            .Replace(".index", "");
    }

    public string GetSchemasDirectoryName()
    {
        return SchemaDirectory;
    }
}