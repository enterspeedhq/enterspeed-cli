using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.SchemaService;

public class SchemaNameService : ISchemaNameService
{
    public bool IsDirectorySchemaName(string schemaName)
    {
        return schemaName.Contains('/');
    }

    public string BuildNewSchemaName(string existingSchemaName, string relativeDirectoryPathOnDisk)
    {
        var lastSegment = Path.GetFileName(existingSchemaName.TrimEnd(Path.DirectorySeparatorChar));
        if (!string.IsNullOrEmpty(relativeDirectoryPathOnDisk))
        {
            return relativeDirectoryPathOnDisk + Path.DirectorySeparatorChar + lastSegment;
        }

        return lastSegment;
    }

    public string GetSchemaName(SchemaFile schemaFile)
    {
        if (!string.IsNullOrEmpty(schemaFile.RelativeSchemaDirectory))
        {
            return Path.Combine(schemaFile.RelativeSchemaDirectory, schemaFile.Alias);
        }

        return schemaFile.Alias;
    }
}