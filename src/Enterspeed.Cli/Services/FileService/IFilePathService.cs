namespace Enterspeed.Cli.Services.FileService;

public interface IFilePathService
{
    string GetRootDirectoryPath(string schemasDirectory);
    string GetRelativeSchemaDirectoryPath(string currentSchemaFilePath, string schemasDirectory);
    string GetDirectoryPathBySchemaName(string schemaName, string schemasDirectory);
}