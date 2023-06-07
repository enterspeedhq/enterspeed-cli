namespace Enterspeed.Cli.Services.IngestService;

public interface IIngestService
{
    Task<bool> Ingest(string filePath, string apiKey, bool useFilenameAsId);
    Task<bool> Delete(string id, string apiKey);
}