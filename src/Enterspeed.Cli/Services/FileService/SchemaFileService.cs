using System.Text.Json;
using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Services.FileService
{
    public class SchemaFileService : ISchemaFileService
    {
        private const string SchemaDirectory = "schemas";
        private static bool SchemaFolderExist => Directory.Exists(SchemaDirectory);

        public void CreateSchema(string alias, int version)
        {
            if (!SchemaFolderExist)
            {
                Directory.CreateDirectory(SchemaDirectory);
            }

            using (var fs = File.Create(GetSchemaFilePath(alias)))
            {
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(new SchemaBaseProperties()
                {
                    Properties = new object(),
                    Triggers = new object()
                });

                fs.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        public SchemaBaseProperties GetSchema(string alias, string filePath = null)
        {
            var schemaFile = GetSchemaFileContent(alias, filePath);
            return JsonSerializer.Deserialize<SchemaBaseProperties>(schemaFile);
        }

        private string GetSchemaFileContent(string alias, string filePath = null)
        {
            var schemaFilePath = filePath ?? GetSchemaFilePath(alias);
            var schemaFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), schemaFilePath));
            return schemaFile;
        }

        private string GetSchemaFilePath(string alias)
        {
            return $"{SchemaDirectory}/{alias}.json";
        }

        public bool ValidateSchemaOnDisk(string externalSchema, string schemaAlias)
        {
            var local = GetSchemaFileContent(schemaAlias);

            // TODO : Can we do this in a better way?
            local = local.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            return local == externalSchema;
        }
    }
}
