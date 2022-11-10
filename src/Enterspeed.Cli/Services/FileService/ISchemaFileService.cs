using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService;

public interface ISchemaFileService
{
    void CreateSchema(string alias, int version);
    SchemaBaseProperties GetSchema(string alias, string filePath = null);
    bool ValidateSchemaOnDisk(string externalSchema, string schemaAlias);
}