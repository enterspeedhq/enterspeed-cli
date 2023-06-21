using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Extensions;

public static class SchemaTypeExtensions
{
    public static string ToApiString(this SchemaType schemaType)
    {
        return schemaType.ToString().ToLower();
    }
}