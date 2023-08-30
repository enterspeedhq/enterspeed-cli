using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Cli.Api.MappingSchema.Models;
using Enterspeed.Cli.Constants;
using Enterspeed.Cli.Domain;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.FileService.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.FileService;

public class SchemaFileService : ISchemaFileService
{
    private readonly ILogger<SchemaFileService> _logger;
    private const string SchemaDirectory = "schemas";
    // logic require partial folder to be a subfolder of normal schema folder
    private const string PartialSchemaDirectory = $"{SchemaDirectory}/partials";

    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SchemaFileService(ILogger<SchemaFileService> logger)
    {
        _logger = logger;
    }

    public void CreateSchema(string alias, SchemaType schemaType, MappingSchemaVersion version = null)
    {
        EnsureSchemaFolders();

        if (SchemaExists(alias))
        {
            _logger.LogWarning($"Schema {alias} exists on disk. Recreating.");
            DeleteSchema(alias);
        }

        if (version != null)
        {
            using (var fs = File.Create(GetRelativeFilePath(alias, schemaType, version.Format)))
            {
                if (version.Format.Equals(SchemaConstants.JsFormat))
                {
                    CreateSchema(version, fs);
                }
                else
                {
                    CreateSchema(schemaType, version, fs);
                }
            }
        }
    }

    private void CreateSchema(MappingSchemaVersion schemaVersion, FileStream fs)
    {
        _logger.LogInformation("Creating javascript schema");

        var decoded = Convert.FromBase64String(schemaVersion.Data);
        fs.Write(decoded, 0, decoded.Length);
    }

    private void CreateSchema(SchemaType schemaType, MappingSchemaVersion schemaVersion, FileStream fs)
    {
        if (schemaVersion.Data == null)
        {
            var emptyContent = schemaType == SchemaType.Partial
                ? new SchemaBaseProperties { Properties = new() }
                : new SchemaBaseProperties { Properties = new(), Triggers = new() };

            schemaVersion.Data = JsonSerializer.Serialize(emptyContent);
        }

        _logger.LogInformation("Creating json schema");

        var content = JsonSerializer.Deserialize<SchemaBaseProperties>(schemaVersion.Data, SerializerOptions);
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(content, SerializerOptions);
        fs.Write(jsonBytes, 0, jsonBytes.Length);
    }

    public SchemaFile GetSchema(string alias, string filePath = null)
    {
        var schemaFilePath = filePath ?? GetFile(alias);
        var schemaFolderName = Path.GetFileName(Path.GetDirectoryName(schemaFilePath));
        var partialSchemaFolderName = Path.GetFileName(Path.GetDirectoryName($"{PartialSchemaDirectory}/"));
        var schemaContent = GetSchemaContent(alias, schemaFilePath);

        var schemaType = schemaFolderName == partialSchemaFolderName ? SchemaType.Partial : SchemaType.Normal;
        var schemaFormat = schemaFilePath.EndsWith(".js") ? SchemaConstants.JsFormat : SchemaConstants.JsonFormat;

        object content;
        if (schemaFormat.Equals(SchemaConstants.JsFormat))
        {
            content = schemaContent;
        }
        else
        {
            content = JsonSerializer.Deserialize<SchemaBaseProperties>(schemaContent, SerializerOptions);
        }


        return new SchemaFile(alias, schemaType, content, schemaFormat);
    }

    public IList<SchemaFile> GetAllSchemas()
    {
        EnsureSchemaFolders();

        var filePaths = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), SchemaDirectory), "*", SearchOption.AllDirectories);

        return filePaths.Select(filePath =>
            {
                var alias = GetAliasFromFilePath(filePath);

                return GetSchema(alias, filePath);
            })
            .ToList();
    }

    public bool SchemaValid(string externalSchema, string schemaAlias)
    {
        if (!SchemaExists(schemaAlias))
        {
            return false;
        }

        var localSchema = GetSchemaContent(schemaAlias);
        return CompareSchemaContent(externalSchema, localSchema);
    }

    public bool SchemaExists(string alias)
    {
        return GetFile(alias) is not null;
    }

    private static void EnsureSchemaFolders()
    {
        if (!Directory.Exists(SchemaDirectory))
        {
            Directory.CreateDirectory(SchemaDirectory);
        }

        if (!Directory.Exists(PartialSchemaDirectory))
        {
            Directory.CreateDirectory(PartialSchemaDirectory);
        }
    }

    private static void DeleteSchema(string alias)
    {
        var file = GetFile(alias);
        if (file is not null)
        {
            File.Delete(file);
        }
    }

    private static string GetSchemaContent(string alias, string filePath = null)
    {
        var schemaFilePath = filePath ?? GetFile(alias);
        return File.ReadAllText(schemaFilePath);
    }

    private static string GetRelativeFilePath(string alias, SchemaType schemaType, string format)
    {
        return schemaType == SchemaType.Normal
            ? Path.Combine(SchemaDirectory, GetFileName(alias, format))
            : Path.Combine(PartialSchemaDirectory, GetFileName(alias, format));
    }

    private static string GetFileName(string alias, string format)
    {
        if (format.Equals(SchemaConstants.JsFormat))
        {
            return $"{alias}.js";
        }

        return $"{alias}.json";
    }

    private static string GetFile(string alias)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var searchDirectory = Path.Combine(currentDirectory, SchemaDirectory);

        return Directory.GetFiles(searchDirectory, alias + ".json", SearchOption.AllDirectories).FirstOrDefault() ??
               Directory.GetFiles(searchDirectory, alias + ".js", SearchOption.AllDirectories).FirstOrDefault();
    }

    private static string GetAliasFromFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }

    private bool CompareSchemaContent(string schema1, string schema2)
    {
        var comparer = new JsonElementComparer();
        try
        {
            using var doc1 = JsonDocument.Parse(schema1);
            using var doc2 = JsonDocument.Parse(schema2);
            return comparer.Equals(doc1.RootElement, doc2.RootElement);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not deserialize schema! {ex.Message}");
        }

        return false;
    }
}