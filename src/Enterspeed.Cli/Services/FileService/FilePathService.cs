namespace Enterspeed.Cli.Services.FileService;

public class FilePathService : IFilePathService
{
    public string GetRootDirectoryPath(string schemasDirectory)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), schemasDirectory);
    }

    private static string GetRelativeDirectoryPathBySchemaName(string schemaName)
    {
        return Path.GetDirectoryName(schemaName.TrimEnd('/'));
    }

    private static string GetDirectoryPathByFilePath(string filePath)
    {
        return Path.GetDirectoryName(filePath);
    }

    public string GetRelativeSchemaDirectoryPath(string currentSchemaFilePath, string schemasDirectory)
    {
        var rootSchemaDirectoryPath = GetRootDirectoryPath(schemasDirectory);
        var currentSchemaDirectoryPath = GetDirectoryPathByFilePath(currentSchemaFilePath);
        var relativeSchemaDirectoryPath = currentSchemaDirectoryPath?.Replace(rootSchemaDirectoryPath, "");

        if (!string.IsNullOrEmpty(relativeSchemaDirectoryPath) && relativeSchemaDirectoryPath.StartsWith(Path.DirectorySeparatorChar))
        {
            relativeSchemaDirectoryPath = relativeSchemaDirectoryPath.Remove(0, 1);
        }

        return relativeSchemaDirectoryPath;
    }

    public string GetDirectoryPathBySchemaName(string schemaName, string schemasDirectory)
    {
        var currentSchemaPathRelative = GetRelativeDirectoryPathBySchemaName(schemaName);
        var schemaDirectoryPath = Path.Combine(GetRootDirectoryPath(schemasDirectory), currentSchemaPathRelative);
        return schemaDirectoryPath;
    }
}