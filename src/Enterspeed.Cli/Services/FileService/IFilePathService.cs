namespace Enterspeed.Cli.Services.FileService;

public interface IFilePathService
{
    string GetRootDirectoryPath();
    string GetRelativeSchemaDirectoryPath(string currentSchemaFilePath);
    string GetAliasFromFilePath(string filePath);
    string GetDirectoryPathBySchemaName(string schemaName);
    string GetRelativeRootDirectoryPath();
}