using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Services.IngestService;

public class IngestService : IIngestService
{
    private readonly HttpClient _httpClient;

    private readonly ILogger<IngestService> _logger;
    public IngestService(ILogger<IngestService> logger, ISettingsService settingsService)
    {
        _logger = logger;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(settingsService.GetSettings().IngestApiUri),
        };
    }

    public async Task<bool> Ingest(string filePath, string apiKey, bool useFilenameAsId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

        var attr = File.GetAttributes(filePath);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            _logger.LogInformation($"Ingesting files from: {filePath}");
            Console.WriteLine($"Ingesting files from: {filePath}");
            foreach (var file in Directory.EnumerateFiles(filePath, "*.json"))
            {
                await IngestFile(file, useFilenameAsId);
            }
        }
        else
        {
            _logger.LogInformation($"Ingesting file: {filePath}");
            Console.WriteLine($"Ingesting file: {filePath}");
            await IngestFile(filePath, useFilenameAsId);
        }

        return true;
    }

    public async Task<bool> Delete(string id, string apiKey)
    {
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        var responseMessage = await Delete(id);

        if (responseMessage.IsSuccessStatusCode)
        {
            return true;
        }

        var err = await new IngestResponse<IngestErrorResponse>().GetMessage(responseMessage);
        _logger.LogError($"Failed to delete: {id}. {err.Message}");
        return false;
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
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var fileData = JsonSerializer.Deserialize<SourceEntityInput>(json, jsonOptions)!;
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
            Console.WriteLine($"Ingest: {id} Ok, {rr.Message}");
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

    private async Task<HttpResponseMessage> Delete(string id)
    {
        return await _httpClient.DeleteAsync(id);
    }

    public class IngestResponse<T> where T : class
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
}