using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Domain.JsonConverters;

public class TenantIdJsonConverter : JsonConverter<TenantId>
{
    public override TenantId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        TenantId tenantIdValue,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(tenantIdValue.IdValue);
    }
}