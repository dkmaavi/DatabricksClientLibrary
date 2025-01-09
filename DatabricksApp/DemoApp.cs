using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
public class DemoApp
{
    private readonly IDatabricksClient client;
    private readonly ILogger<DemoApp> logger;

    public DemoApp(IDatabricksClient client, ILogger<DemoApp> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task RunAsync()
    {
        try
        {
            logger.LogInformation("Starting App");

            await ExecuteQuery();

            logger.LogInformation("Completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred");
        }
    }

    private async Task ExecuteQuery()
    {
        var query = new StatementQuery
        {
            Statement = @"SELECT * FROM software where tenantId = :tenantId;",
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

        var softwares = await client.GetResultAsync<Software>(query);

        foreach (var software in softwares)
        {
            logger.LogInformation("Software: {executable} - {Product} - {Publishre}",
                software.Executable, software.Product, software.Publisher);
        }
    }



}
