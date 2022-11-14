using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService;

public interface ISchemaFileService
{
    void CreateSchema(string alias, string content = null);
    SchemaBaseProperties GetSchema(string alias, string filePath = null);
    bool SchemaValid(string externalSchema, string schemaAlias);
}