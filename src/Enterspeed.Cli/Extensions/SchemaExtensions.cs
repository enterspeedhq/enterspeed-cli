using Enterspeed.Cli.Constants;
using Enterspeed.Cli.Services.FileService.Models;
using Enterspeed.Cli.Services.FileService;
using System.Text;
using System.Text.Json;

namespace Enterspeed.Cli.Extensions
{
    public static class SchemaExtensions
    {
        public static object GetSchemaContent(this SchemaFile schemaFile)
        {
            return schemaFile.Format.Equals(SchemaConstants.JavascriptFormat)
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(schemaFile.Content?.ToString() ?? string.Empty))
                : JsonSerializer.SerializeToDocument(schemaFile.Content, SchemaFileService.SerializerOptions);
        }

        public static bool SchemaIsInDirectory(string schemaName)
        {
            return schemaName.Contains('/');
        }

        public static string BuildNewSchemaName(string existingSchemaName, string relativeDirectoryPathOndisk)
        {
            var lastSegment = Path.GetFileName(existingSchemaName.TrimEnd(Path.DirectorySeparatorChar));
            var name = relativeDirectoryPathOndisk + Path.DirectorySeparatorChar + lastSegment;

            return name;
        }
    }
}