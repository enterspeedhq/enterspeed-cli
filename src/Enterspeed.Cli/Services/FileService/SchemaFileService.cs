using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Cli.Api.MappingSchema.Models;
using Enterspeed.Cli.Constants;
using Enterspeed.Cli.Domain;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.FileService.Models;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.FileService;

public class SchemaFileService : ISchemaFileService
{
    private readonly ILogger<SchemaFileService> _logger;

    private const string SchemaDirectory = "schemas";

    private const string DefaultJsContent =
        "/** @type {Enterspeed.FullSchema} */\nexport default {\n  triggers: function(context) {\n    // Example that triggers on 'mySourceEntityType' in 'mySourceGroupAlias', adjust to match your own values\n    // See documentation for triggers here: https://docs.enterspeed.com/reference/js/triggers\n    context.triggers('mySourceGroupAlias', ['mySourceEntityType'])\n  },\n  routes: function(sourceEntity, context) {\n    // Example that generates a handle with the value of 'my-handle' to use when fetching the view from the Delivery API\n    // See documentation for routes here: https://docs.enterspeed.com/reference/js/routes\n    context.handle('my-handle')\n  },\n  properties: function (sourceEntity, context) {\n    // Example that returns all properties from the source entity to the view\n    // See documentation for properties here: https://docs.enterspeed.com/reference/js/properties\n    return sourceEntity.properties\n  }\n}";

    private const string DefaultJsPartialContent =
        "/** @type {Enterspeed.PartialSchema} */\nexport default {\n  properties: function (input, context) {\n    // Example that returns all properties from the input object to the view\n    // See documentation for properties here: https://docs.enterspeed.com/reference/js/properties\n    return input\n  }\n}";

    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SchemaFileService(ILogger<SchemaFileService> logger)
    {
        _logger = logger;
    }

    public void CreateSchema(string alias, SchemaType schemaType, MappingSchemaVersion version, string schemaName)
    {
        EnsureSchemaFolders(schemaName);

        if (SchemaExists(alias))
        {
            _logger.LogWarning($"Schema {alias} exists on disk. Recreating.");
            DeleteSchema(alias);
        }

        var filePath = GetFilePath(schemaName, alias, schemaType, version.Format);
        using var fs = File.Create(filePath);
        if (version.Format.Equals(SchemaConstants.JavascriptFormat))
        {
            CreateJavascriptSchemaFile(schemaType, version, fs);
        }
        else
        {
            CreateJsonSchemaFile(schemaType, version, fs);
        }
    }

    private void CreateJavascriptSchemaFile(SchemaType schemaType, MappingSchemaVersion schemaVersion, FileStream fs)
    {
        _logger.LogInformation("Creating javascript schema");

        // Version does not have any data. Assign default js setup for js schemas instead as a temp fix. 
        if (schemaVersion.Data == null)
        {
            var byteArray = Encoding.UTF8.GetBytes(schemaType == SchemaType.Partial ? DefaultJsPartialContent : DefaultJsContent);
            fs.Write(byteArray, 0, byteArray.Length);
        }
        else
        {
            var decoded = Convert.FromBase64String(schemaVersion.Data);
            fs.Write(decoded, 0, decoded.Length);
        }
    }

    private void CreateJsonSchemaFile(SchemaType schemaType, MappingSchemaVersion schemaVersion, Stream fs)
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
        var currentSchemaFilePath = filePath ?? GetFile(alias);
        var schemaFormat = currentSchemaFilePath.EndsWith(".js") ? SchemaConstants.JavascriptFormat : SchemaConstants.JsonFormat;

        var schemaContent = GetSchemaContent(alias, currentSchemaFilePath);
        object content;

        if (schemaFormat.Equals(SchemaConstants.JavascriptFormat))
        {
            content = schemaContent;
        }
        else
        {
            content = JsonSerializer.Deserialize<SchemaBaseProperties>(schemaContent, SerializerOptions);
        }

        // Get relative path for schema
        var currentSchemaDirectoryPath = Path.GetDirectoryName(currentSchemaFilePath);
        var relativeSchemaDirectoryPath = GetRelativeSchemaPath(currentSchemaDirectoryPath);
        if (!string.IsNullOrEmpty(relativeSchemaDirectoryPath) && relativeSchemaDirectoryPath.StartsWith(Path.DirectorySeparatorChar))
        {
            relativeSchemaDirectoryPath = relativeSchemaDirectoryPath.Remove(0, 1);
        }

        var schemaType = currentSchemaFilePath.Contains(SchemaType.Partial.ToString().ToLowerInvariant()) ? SchemaType.Partial : SchemaType.Normal;
        return new SchemaFile(alias, schemaType, content, schemaFormat, relativeSchemaDirectoryPath);
    }

    private static string GetRootSchemaPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), SchemaDirectory);
    }

    private static string GetRelativeSchemaPath(string currentSchemaDirectoryPath)
    {
        var rootSchemaDirectoryPath = GetRootSchemaPath();
        var relativeSchemaDirectoryPath = currentSchemaDirectoryPath?.Replace(rootSchemaDirectoryPath, "");
        return relativeSchemaDirectoryPath;
    }

    private static string GetRelativeSchemaPathByName(string schemaName)
    {
        return Path.GetDirectoryName(schemaName.TrimEnd(Path.DirectorySeparatorChar));
    }

    public IList<SchemaFile> GetAllSchemas()
    {
        EnsureSchemaFolders();

        var filePaths = Directory.GetFiles(GetRootSchemaPath(), "*", SearchOption.AllDirectories);
        return filePaths.Select(filePath =>
            {
                var alias = GetAliasFromFilePath(filePath);
                return GetSchema(alias, filePath);
            })
            .ToList();
    }

    public bool SchemaValid(MappingSchemaVersion externalSchema, string schemaAlias)
    {
        if (!SchemaExists(schemaAlias))
        {
            return false;
        }

        var localSchema = GetSchemaContent(schemaAlias);

        return externalSchema.Format.Equals(SchemaConstants.JsonFormat)
            ? CompareJsonSchemas(externalSchema.Data, localSchema)
            : CompareJavascriptSchemas(externalSchema.Data, localSchema);
    }

    public bool SchemaExists(string alias)
    {
        return GetFile(alias) is not null;
    }

    private static void EnsureSchemaFolders(string schemaName = null)
    {
        if (!Directory.Exists(SchemaDirectory))
        {
            Directory.CreateDirectory(SchemaDirectory);
        }

        if (schemaName != null)
        {
            if (SchemaExtensions.SchemaIsInDirectory(schemaName))
            {
                var schemasRootPath = GetRootSchemaPath();
                var currentSchemaPathRelative = GetRelativeSchemaPathByName(schemaName);
                if (!string.IsNullOrEmpty(currentSchemaPathRelative))
                {
                    var schemaDirectoryPath = Path.Combine(schemasRootPath, currentSchemaPathRelative);
                    if (!Directory.Exists(schemaDirectoryPath))
                    {
                        Directory.CreateDirectory(schemaDirectoryPath);
                    }
                }
            }
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

    private static string GetFilePath(string schemaName, string alias, SchemaType schemaType, string format)
    {
        // Folder structure is defined by name, therefore name is passed as parameter
        if (SchemaExtensions.SchemaIsInDirectory(schemaName))
        {
            // Removing last part of name to get the relative directory path
            var currentSchemaPathRelative = GetRelativeSchemaPathByName(schemaName);
            if (currentSchemaPathRelative != null)
            {
                var schemaDirectoryPath = Path.Combine(GetRootSchemaPath(), currentSchemaPathRelative);
                var fullFilePath = Path.Combine(schemaDirectoryPath, GetFileName(alias, format, schemaType));
                return fullFilePath;
            }
        }

        return Path.Combine(SchemaDirectory, GetFileName(alias, format, schemaType));
    }

    private static string GetFileName(string alias, string format, SchemaType schemaType)
    {
        var schemaTypeName = schemaType switch
        {
            SchemaType.Normal => "full",
            SchemaType.Partial => "partial",
            _ => throw new ArgumentOutOfRangeException(nameof(schemaType), schemaType, null)
        };

        var schemaName = $"{alias}.{schemaTypeName}";
        return format.Equals(SchemaConstants.JavascriptFormat) ? $"{schemaName}.js" : $"{schemaName}.json";
    }

    private static string GetFile(string alias)
    {
        var searchDirectory = GetRootSchemaPath();

        if (!Directory.Exists(searchDirectory))
        {
            return null;
        }

        return Directory.GetFiles(searchDirectory, alias + "*.json", SearchOption.AllDirectories).FirstOrDefault() ??
               Directory.GetFiles(searchDirectory, alias + "*.js", SearchOption.AllDirectories).FirstOrDefault();
    }

    private static string GetAliasFromFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath).Replace("." + SchemaType.Partial.ToString().ToLowerInvariant(), "");
    }

    private static bool CompareJavascriptSchemas(string externalSchema, string localSchema)
    {
        var externalSchemaDecoded = Convert.FromBase64String(externalSchema);
        var externalSchemaDecodedString = Encoding.UTF8.GetString(externalSchemaDecoded);

        return externalSchemaDecodedString.Equals(localSchema);
    }

    private bool CompareJsonSchemas(string schema1, string schema2)
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