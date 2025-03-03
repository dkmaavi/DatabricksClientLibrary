﻿namespace Tachyon.Server.Common.DatabricksClient.Models.Configuration
{
    public class HttpClientSettings
    {
        public string BaseUrl { get; set; }
        public string BearerToken { get; set; }
        public int HttpTimeout { get; set; } = 300;
    }
}