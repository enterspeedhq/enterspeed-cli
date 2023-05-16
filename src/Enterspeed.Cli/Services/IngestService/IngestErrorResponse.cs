namespace Enterspeed.Cli.Services.IngestService;

public class IngestErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
    public Dictionary<string, string> Errors { get; set; }
}