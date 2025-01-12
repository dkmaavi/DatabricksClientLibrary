namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Handlers
{
    public interface IDatabricksErrorHandler
    {
        Task HandleErrorAsync(Exception ex, CancellationToken cancellationToken);
    }
}