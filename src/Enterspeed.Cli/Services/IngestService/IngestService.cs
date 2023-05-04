using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.IngestService;

public class IngestService : IIngestService
{
    private const string IngestUrl = "https://api.enterspeed.com/ingest/v2/";
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(IngestUrl),
    };

    private readonly ILogger<IngestService> _logger;
    public IngestService(ILogger<IngestService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Ingest(string filePath, string apiKey, bool useFilenameAsId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

        var attr = File.GetAttributes(filePath);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            _logger.LogInformation($"Ingesting files from: {filePath}");
            foreach (var file in Directory.EnumerateFiles(filePath, "*.json"))
            {
                await IngestFile(file, useFilenameAsId);
            }
        }
        else
        {
            _logger.LogInformation($"Ingesting file: {filePath}");
            await IngestFile(filePath, useFilenameAsId);
        }

        return true;
    }

    private async Task IngestFile(string filename, bool useFilenameAsId)
    {
        var json = await File.ReadAllTextAsync(filename);

        string id;
        if (useFilenameAsId)
        {
            id = Path.GetFileNameWithoutExtension(filename);
        }
        else
        {
            var fileData = JsonSerializer.Deserialize<SourceEntityInput>(json)!;
            id = fileData.Id;
        }

        if (string.IsNullOrEmpty(id))
        {
            _logger.LogError($"Id missing from {filename}");
            return;
        }

        _logger.LogInformation($"Ingesting: {id}");

        var responseMessage = await Post(id, json);
        if (responseMessage.IsSuccessStatusCode)
        {
            var rr = await new IngestResponse<IngestOkResponse>().GetMessage(responseMessage);
            _logger.LogInformation($"Ingest: {id} Ok, {rr.Message}");
        }
        else
        {
            var err = await new IngestResponse<IngestErrorResponse>().GetMessage(responseMessage);
            if (err != null)
            {
                var allErrors = string.Join(", ", err.Errors.Select(e => $"{e.Key}:{e.Value}"));
                _logger.LogError($"Ingest: {id} Error: {err.Message} {err.ErrorCode}. Errors: {allErrors}");
            }
            else
            {
                _logger.LogError($"Ingest: {id} Error: {responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }
        }
    }

    private async Task<HttpResponseMessage> Post(string id, string json)
    {
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(id, data);
        return response;
    }

   

    class IngestResponse<T> where T : class
    {
        public async Task<T> GetMessage(HttpResponseMessage responseMessage) 
        {
            var contentStream = await responseMessage.Content.ReadAsStreamAsync();
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(contentStream, new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    class IngestOkResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

    class IngestErrorResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}