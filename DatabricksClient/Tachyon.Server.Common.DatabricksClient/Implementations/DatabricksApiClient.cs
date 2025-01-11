namespace Tachyon.Server.Common.DatabricksClient.Implementations
{
    using System;
    using Microsoft.Extensions.Logging;
    using Tachyon.Server.Common.DatabricksClient.Abstractions;
    using Tachyon.Server.Common.DatabricksClient.Exceptions;
    using Tachyon.Server.Common.DatabricksClient.Implementations.Interceptors;
    using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
    using Tachyon.Server.Common.DatabricksClient.Models.Enums;
    using Tachyon.Server.Common.DatabricksClient.Models.Request;
    using Tachyon.Server.Common.DatabricksClient.Models.Response;
    using Tachyon.Server.Common.DatabricksClient.Utilities;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
    using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;

    internal class DatabricksApiClient : IDatabricksApiClient
    {
        private readonly IDatabricksCommunicationService databricksCommunicationService;
        private readonly IDatabricksResultHandler databricksResultHandler;
        private readonly IDatabricksErrorHandler databricksErrorHandler;
        private readonly DatabricksInterceptorProcessor interceptorProcessor;
        private readonly ResilienceSettings resilienceSettings;
        private readonly ILogger<DatabricksApiClient> logger;
        public DatabricksApiClient(IDatabricksCommunicationService databricksCommunicationService, IDatabricksResultHandler databricksResultHandler,
            IDatabricksErrorHandler databricksErrorHandler, DatabricksInterceptorProcessor interceptorProcessor, ResilienceSettings resilienceSettings, ILogger<DatabricksApiClient> logger)
        {
            this.databricksCommunicationService = databricksCommunicationService;
            this.databricksCommunicationService = databricksCommunicationService;
            this.databricksResultHandler = databricksResultHandler;
            this.databricksErrorHandler = databricksErrorHandler;
            this.interceptorProcessor = interceptorProcessor;
            this.resilienceSettings = resilienceSettings;
            this.logger = logger;
        }

        public async Task<List<T>> GetResultAsync<T>(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            var response = await ExecuteQueryAsync(statementQuery, cancellationToken);
            return await databricksResultHandler.HandleResultAsync<T>(response, cancellationToken);
        }

        public async Task<StatementResult> ExecuteQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Executing databricks query with Id - {QueryId}", statementQuery.QueryId);

                await interceptorProcessor.BeforeRequestAsync(statementQuery);

                var result = await ExecuteWithPollingAsync(statementQuery, cancellationToken);

                await interceptorProcessor.AfterRequestAsync(result);

                logger.LogInformation("Successfully executed databricks query with Id - {QueryId}", statementQuery.QueryId);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to execute databricks query with Id - {QueryId}", statementQuery.QueryId);
                await databricksErrorHandler.HandleErrorAsync(ex, cancellationToken);
                throw;
            }
        }

        private async Task<StatementResult> ExecuteWithPollingAsync(StatementQuery statementQuery, CancellationToken cancellationToken)
        {
            using var executionTimer = new ExecutionTimer();

            var result = await databricksCommunicationService.SendStatementQueryAsync(statementQuery, cancellationToken);

            while (result.Status.State is State.Running or State.Pending) //TODO -  need to handle response with multiple chunks
            {
                if (executionTimer.HasExceededTimeout(resilienceSettings.QueryTimeout))
                {
                    throw new DatabricksException(ErrorCode.TIMEOUT, $"Databricks query execution exceeded timeout of {resilienceSettings.QueryTimeout} seconds");
                }

                logger.LogDebug("Running databricks query for statement - {StatementId} with status - {State}",
                    result.StatementId, result.Status.State);

                await Task.Delay(TimeSpan.FromSeconds(resilienceSettings.PollingInterval), cancellationToken);

                result = await databricksCommunicationService.GetStatementResultAsync(result.StatementId, cancellationToken);
            }

            return result;
        }
    }
}