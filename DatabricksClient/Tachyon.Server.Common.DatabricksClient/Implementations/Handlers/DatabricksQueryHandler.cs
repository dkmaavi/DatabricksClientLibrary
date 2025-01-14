using Microsoft.Extensions.Logging;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers;
using Tachyon.Server.Common.DatabricksClient.Abstractions.Services;
using Tachyon.Server.Common.DatabricksClient.Exceptions;
using Tachyon.Server.Common.DatabricksClient.Models.Configuration;
using Tachyon.Server.Common.DatabricksClient.Models.Enums;
using Tachyon.Server.Common.DatabricksClient.Models.Request;
using Tachyon.Server.Common.DatabricksClient.Models.Response;
using Tachyon.Server.Common.DatabricksClient.Utilities;

namespace Tachyon.Server.Common.DatabricksClient.Implementations.Handlers
{
    internal class DatabricksQueryHandler : IDatabricksQueryHandler
    {
        private readonly IDatabricksCommunicationService communicationService;
        private readonly ResilienceSettings resilienceSettings;
        private readonly ILogger<DatabricksQueryHandler> logger;

        public DatabricksQueryHandler(
            IDatabricksCommunicationService communicationService,
            ResilienceSettings resilienceSettings,
            ILogger<DatabricksQueryHandler> logger)
        {
            this.communicationService = communicationService;
            this.resilienceSettings = resilienceSettings;
            this.logger = logger;
        }

        public async Task<StatementResult> HandleQueryAsync(StatementQuery statementQuery, CancellationToken cancellationToken)
        {
            using var executionTimer = new ExecutionTimer();

            var result = await communicationService.SendStatementQueryAsync(statementQuery, cancellationToken);

            result = await PollQueryCompletionAsync(result, executionTimer, cancellationToken);

            return result;
        }

        private async Task<StatementResult> PollQueryCompletionAsync(StatementResult result, ExecutionTimer executionTimer, CancellationToken cancellationToken)
        {
            while (result.Status.State is State.Running or State.Pending)
            {
                if (executionTimer.HasExceededTimeout(resilienceSettings.QueryTimeout))
                {
                    throw new DatabricksException(ErrorCode.TIMEOUT, $"Databricks query execution exceeded timeout of {resilienceSettings.QueryTimeout} seconds.");
                }

                logger.LogDebug("Polling Databricks query. Statement Id: {StatementId}, Current State: {State}",
                    result.StatementId, result.Status.State);

                await Task.Delay(TimeSpan.FromSeconds(resilienceSettings.PollingInterval), cancellationToken);

                result = await communicationService.GetStatementResultAsync(result.StatementId, cancellationToken);
            }

            return result;
        }
    }
}
