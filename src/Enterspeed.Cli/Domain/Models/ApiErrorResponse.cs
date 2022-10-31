using System.Dynamic;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Domain.Models;

public class ApiErrorResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("detail")]
    public string Detail { get; set; }
    [JsonPropertyName("errors")]
    public ExpandoObject Errors { get; set; }
    [JsonPropertyName("warnings")]
    public ExpandoObject Warnings { get; set; }
}
