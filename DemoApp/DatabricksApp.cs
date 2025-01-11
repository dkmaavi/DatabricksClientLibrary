using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
namespace DemoApp
{
    public class DatabricksApp
    {
        private readonly IDatabricksApiClient client;
        private readonly ILogger<DatabricksApp> logger;

        public DatabricksApp(IDatabricksApiClient client, ILogger<DatabricksApp> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task RunAsync()
        {
            try
            {
                logger.LogInformation("Starting App");

                await ExecuteGetQuery();

               // await ExecuteGetQuery();

                //await ExecuteQuery();

                logger.LogInformation("Completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred");
            }
        }

        private async Task ExecuteGetQuery()
        {
            var query = new StatementQuery
            {
                Statement =  @"SELECT * FROM software_main;",
                Parameters = new List<StatementQueryParameter>
            {
                new StatementQueryParameter
                {
                    Name = "tenantId",
                    Value = "0fbe3331-2f13-45df-b3f3-437d83e9339c",
                    Type  ="STRING"
                }
            }
            };//

            var softwares = await client.GetResultAsync<Software>(query);

            foreach (var software in softwares)
            {
                logger.LogInformation("Software: {executable} - {Product} - {Publishre}",
                    software.Executable, software.Product, software.Publisher);
            }
        }

        private async Task ExecuteQuery()
        {
            var query = new StatementQuery
            {
                Statement = @"INSERT INTO matchedsoftware (TenantId, SoftwareHash, Vendor, Title, IsMatched, IsPartiallyMatched, IsPartiallyVersionMatched) VALUES ('5c00ccf8-fd11-439e-b667-41608e1b4d0b', 'DummyCode', 'Vendor','Title', TRUE, TRUE,TRUE);",
                Parameters = new List<StatementQueryParameter>
            {
                new StatementQueryParameter
                {
                    Name = "tenantId",
                    Value = "0fbe3331-2f13-45df-b3f3-437d83e9339c",
                    Type  ="STRING"
                }
            }
            };

            await client.ExecuteQueryAsync(query);

            logger.LogInformation("Result saved successfully");
        }
    }
}
