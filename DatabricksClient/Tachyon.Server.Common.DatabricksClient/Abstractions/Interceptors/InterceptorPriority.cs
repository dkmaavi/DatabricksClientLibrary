namespace Tachyon.Server.Common.DatabricksClient.Abstractions.Interceptors
{
    public enum InterceptorPriority
    {
        Normal = 0,
        High = 10,
        Critical = 20,
    }
}