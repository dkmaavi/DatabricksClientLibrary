using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Implementations.Builders;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;

namespace Tachyon.Server.Common.DatabricksClient.Implementations
{
    internal class DatabricksApiClient : IDatabricksApiClient
    {
        private readonly DatabricksPipelineProcessor pipelineProcessor;
        private readonly IDatabricksResultHandler resultHandler;
        private readonly IDatabricksErrorHandler errorHandler;
        private readonly ILogger<DatabricksApiClient> logger;

        public DatabricksApiClient(DatabricksPipelineProcessor pipelineProcessor,
            IDatabricksResultHandler resultHandler, IDatabricksErrorHandler errorHandler, ILogger<DatabricksApiClient> logger)
        {
            this.pipelineProcessor = pipelineProcessor;
            this.resultHandler = resultHandler;
            this.errorHandler = errorHandler;
            this.logger = logger;
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await ExecuteQueryInternalAsync(statementQuery, cancellationToken);

            return await HandleResultInternalAsync<T>(statementQuery.QueryId, response, cancellationToken);
        }

        public async Task<StatementResult> ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            return await ExecuteQueryInternalAsync(statementQuery, cancellationToken);
        }

        private async Task<StatementResult> ExecuteQueryInternalAsync(StatementQuery statementQuery, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Executing Databricks query with Id - {QueryId}", statementQuery.QueryId);

                var result = await pipelineProcessor(statementQuery, cancellationToken);

                logger.LogInformation("Successfully executed Databricks query with Id - {QueryId}", statementQuery.QueryId);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to execute Databricks query with Id - {QueryId}", statementQuery.QueryId);
                await errorHandler.HandleErrorAsync(ex, cancellationToken);
                throw;
            }
        }

        private async Task<List<T>> HandleResultInternalAsync<T>(Guid queryId, StatementResult response, CancellationToken cancellationToken)
        {
            try
            {
                return await resultHandler.HandleResultAsync<T>(response, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle Databricks result with Id - {QueryId}", queryId);
                await errorHandler.HandleErrorAsync(ex, cancellationToken);
                throw;
            }
        }
    }
}
