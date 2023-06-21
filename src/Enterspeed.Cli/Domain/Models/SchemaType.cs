using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SchemaType
{
    Normal,
    Partial
}