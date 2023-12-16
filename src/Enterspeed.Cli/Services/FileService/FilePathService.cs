using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Services.FileService;

public class FilePathService : IFilePathService
{
    private const string SchemaDirectory = "schemas";

    public string GetRelativeRootDirectoryPath()
    {
        return SchemaDirectory;
    }

    public string GetRootDirectoryPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), SchemaDirectory);
    }

    private static string GetRelativeDirectoryPathBySchemaName(string schemaName)
    {
        return Path.GetDirectoryName(schemaName.TrimEnd(Path.DirectorySeparatorChar));
    }

    private static string GetDirectoryPathByFilePath(string filePath)
    {
        return Path.GetDirectoryName(filePath);
    }

    public string GetRelativeSchemaDirectoryPath(string currentSchemaFilePath)
    {
        var rootSchemaDirectoryPath = GetRootDirectoryPath();
        var currentSchemaDirectoryPath = GetDirectoryPathByFilePath(currentSchemaFilePath);
        var relativeSchemaDirectoryPath = currentSchemaDirectoryPath?.Replace(rootSchemaDirectoryPath, "");

        if (!string.IsNullOrEmpty(relativeSchemaDirectoryPath) && relativeSchemaDirectoryPath.StartsWith(Path.DirectorySeparatorChar))
        {
            relativeSchemaDirectoryPath = relativeSchemaDirectoryPath.Remove(0, 1);
        }

        return relativeSchemaDirectoryPath;
    }

    public string GetAliasFromFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath).Replace(".partial", "")
            .Replace(".full", "");
    }

    public string GetDirectoryPathBySchemaName(string schemaName)
    {
        var currentSchemaPathRelative = GetRelativeDirectoryPathBySchemaName(schemaName);
        var schemaDirectoryPath = Path.Combine(GetRootDirectoryPath(), currentSchemaPathRelative);
        return schemaDirectoryPath;
    }
}