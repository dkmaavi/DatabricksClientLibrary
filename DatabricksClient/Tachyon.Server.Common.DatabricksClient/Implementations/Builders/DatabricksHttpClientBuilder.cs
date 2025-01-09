namespace Tachyon.Server.Common.DatabricksClient.Implementations.Builders
{
    using System.Net.Http.Headers;
    using Tachyon.Server.Common.DatabricksClient.Constants;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Builders;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;

    public class DatabricksHttpClientBuilder : IDatabricksHttpClientBuilder
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClientSettings httpSettings;
        public DatabricksHttpClientBuilder(IHttpClientFactory httpClientFactory, HttpClientSettings httpSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpSettings = httpSettings;
        }
        public HttpClient BuildClient()
        {
            var httpClient = httpClientFactory.CreateClient(ApiConstants.HttpClientName);

            httpClient.BaseAddress = new Uri(httpSettings.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpSettings.BearerToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

    }
}
