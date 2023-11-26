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

    // logic require partial folder to be a subfolder of normal schema folder
    private const string PartialSchemaDirectory = $"{SchemaDirectory}/partials";

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

        using var fs = File.Create(GetRelativeFilePath(schemaName, alias, schemaType, version.Format));
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
        var schemaFilePath = filePath ?? GetFile(alias);
        var schemaFolderName = Path.GetFileName(Path.GetDirectoryName(schemaFilePath));
        var partialSchemaFolderName = Path.GetFileName(Path.GetDirectoryName($"{PartialSchemaDirectory}/"));
        var schemaContent = GetSchemaContent(alias, schemaFilePath);

        var schemaType = schemaFolderName == partialSchemaFolderName ? SchemaType.Partial : SchemaType.Normal;
        var schemaFormat = schemaFilePath.EndsWith(".js") ? SchemaConstants.JavascriptFormat : SchemaConstants.JsonFormat;

        object content;
        if (schemaFormat.Equals(SchemaConstants.JavascriptFormat))
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

    public bool SchemaValid(MappingSchemaVersion externalSchema, string schemaAlias)
    {
        if (!SchemaExists(schemaAlias))
        {
            return false;
        }

        var localSchema = GetSchemaContent(schemaAlias);
        return CompareSchemaContent(externalSchema.Data, localSchema, externalSchema.Format);
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

        if (!Directory.Exists(PartialSchemaDirectory))
        {
            Directory.CreateDirectory(PartialSchemaDirectory);
        }

        if (schemaName != null)
        {
            if (SchemaExtensions.SchemaIsInFolder(schemaName))
            {
                // First item in list is the top level folder
                var folders = GetSchemaFolders(schemaName).ToArray();

                var path = Path.Combine(folders);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
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

    private static string GetRelativeFilePath(string schemaName, string alias, SchemaType schemaType, string format)
    {
        if (SchemaExtensions.SchemaIsInFolder(schemaName))
        {
            var path = GetSchemaFolders(schemaName).ToList();
            path.Add(GetFileName(alias, format));

            return Path.Combine(path.ToArray());
        }

        return schemaType == SchemaType.Normal
            ? Path.Combine(SchemaDirectory, GetFileName(alias, format))
            : Path.Combine(PartialSchemaDirectory, GetFileName(alias, format));
    }

    private static string GetFileName(string alias, string format)
    {
        if (format.Equals(SchemaConstants.JavascriptFormat))
        {
            return $"{alias}.js";
        }

        return $"{alias}.json";
    }

    private static string GetFile(string alias)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var searchDirectory = Path.Combine(currentDirectory, SchemaDirectory);

        if (!Directory.Exists(searchDirectory))
        {
            return null;
        }

        return Directory.GetFiles(searchDirectory, alias + ".json", SearchOption.AllDirectories).FirstOrDefault() ??
               Directory.GetFiles(searchDirectory, alias + ".js", SearchOption.AllDirectories).FirstOrDefault();
    }

    private static string GetAliasFromFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }


    private bool CompareSchemaContent(string externalSchema, string localSchema, string schemaFormat)
    {
        return schemaFormat.Equals(SchemaConstants.JsonFormat)
            ? CompareJsonSchemas(externalSchema, localSchema)
            : CompareJavascriptSchemas(externalSchema, localSchema);
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

    private static IEnumerable<string> GetSchemaFolders(string schemaName)
    {
        var names = schemaName.Split('/');
        // Get all folders except the last one
        var schemaFolders = new List<string> { Directory.GetCurrentDirectory(), SchemaDirectory };

        for (var i = 0; i < names.Length - 1; i++)
        {
            schemaFolders.Add(names[i]);
        }

        return schemaFolders;
    }
}