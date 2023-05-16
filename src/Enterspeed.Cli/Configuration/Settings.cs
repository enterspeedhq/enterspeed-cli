﻿namespace Enterspeed.Cli.Configuration
{
    public sealed class Settings
    {
        public string ManagementApiUri { get; set; } = "https://management.enterspeed.com/api/v1/";
        public string IngestApiUri { get; set; } = "https://api.enterspeed.com/ingest/v2/";
    }
}
