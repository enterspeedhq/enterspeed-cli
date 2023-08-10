using Enterspeed.Cli.Api.MappingSchema.Models;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService;

public interface ISchemaFileService
{
    void CreateSchema(string alias, SchemaType schemaType, MappingSchemaVersion version = null);
    SchemaFile GetSchema(string alias, string filePath = null);
    IList<SchemaFile> GetAllSchemas();
    bool SchemaExists(string alias);
    bool SchemaValid(string externalSchema, string schemaAlias);
}