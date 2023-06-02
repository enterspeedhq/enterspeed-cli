using System.Dynamic;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Domain.Models;

public class ApiErrorBaseResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("detail")]
    public string Detail { get; set; }
}

public class ApiErrorResponse : ApiErrorBaseResponse
{
    [JsonPropertyName("errors")]
    public ExpandoObject Errors { get; set; }
    [JsonPropertyName("warnings")]
    public ExpandoObject Warnings { get; set; }
}

public class ApiGroupedErrorResponse : ApiErrorBaseResponse
{
    [JsonPropertyName("errors")]
    public GroupedError[] Errors { get; set; }
    [JsonPropertyName("warnings")]
    public GroupedError[] Warnings { get; set; }
}

public class GroupedError
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("errors")]
    public ExpandoObject Errors { get; set; }
}
