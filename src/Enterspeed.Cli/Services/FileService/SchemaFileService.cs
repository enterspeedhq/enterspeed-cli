using System.Text.Json;
using Enterspeed.Cli.Services.FileService.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.FileService
{
    public class SchemaFileService : ISchemaFileService
    {
        private readonly ILogger<SchemaFileService> _logger;
        private const string SchemaDirectory = "schemas";

        public SchemaFileService(ILogger<SchemaFileService> logger)
        {
            _logger = logger;
        }

        public void CreateSchema(string alias, string content = null)
        {
            EnsureSchemaFolder();

            if (SchemaExists(alias))
            {
                _logger.LogWarning($"Schema {alias} exists on disk. Recreating.");
                DeleteSchema(alias);
            }

            if (content == null)
            {
                CreateEmptySchema(alias);
            }
            else
            {
                CreatePopulatedSchema(alias, content);
            }
        }

        private void CreateEmptySchema(string alias)
        {
            // Create empty schema
            CreateSchema(alias, new SchemaBaseProperties()
            {
                Properties = new(),
                Triggers = new()
            });
        }

        private void CreatePopulatedSchema(string alias, string content)
        {
            CreateSchema(alias, JsonSerializer.Deserialize<SchemaBaseProperties>(content));
        }

        private void CreateSchema(string alias, SchemaBaseProperties content)
        {
            using (var fs = File.Create(GetFilePath(alias)))
            {
                _logger.LogInformation("Creating schema");

                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(content);
                fs.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        public SchemaBaseProperties GetSchema(string alias, string filePath = null)
        {
            var schemaFile = GetSchemaContent(alias, filePath);
            return JsonSerializer.Deserialize<SchemaBaseProperties>(schemaFile);
        }

        public bool ValidateSchema(string externalSchema, string schemaAlias)
        {
            var local = GetSchemaContent(schemaAlias);

            // TODO : Can we do this in a better way?
            local = local.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            return local == externalSchema;
        }

        private void EnsureSchemaFolder()
        {
            if (!Directory.Exists(SchemaDirectory))
            {
                Directory.CreateDirectory(SchemaDirectory);
            }
        }

        private bool SchemaExists(string alias)
        {
            var schemaFilePath = GetFilePath(alias);
            return File.Exists(Path.Combine(Directory.GetCurrentDirectory(), schemaFilePath));
        }

        private void DeleteSchema(string alias)
        {
            var schemaFilePath = GetFilePath(alias);
            File.Delete(schemaFilePath);
        }

        private string GetSchemaContent(string alias, string filePath = null)
        {
            var schemaFilePath = filePath ?? GetFilePath(alias);
            return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), schemaFilePath));
        }

        private string GetFilePath(string alias)
        {
            return $"{SchemaDirectory}/{alias}.json";
        }
    }
}
