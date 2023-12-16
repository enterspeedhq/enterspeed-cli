using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.SchemaService;

public interface ISchemaNameService
{
    bool IsDirectorySchemaName(string schemaName);
    string BuildNewSchemaName(string existingSchemaName, string relativeDirectoryPathOnDisk);
    string GetSchemaName(SchemaFile schemaFile);
}