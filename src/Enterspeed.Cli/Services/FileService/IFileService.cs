namespace Enterspeed.Cli.Services.FileService;

public interface IFileService
{
    void CreateSchema(string schemaAlias, int version, string mappingSchemaGuid);
}