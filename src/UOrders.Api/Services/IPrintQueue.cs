namespace UOrders.Api.Services;

public interface IPrintQueue
{
    Task<int> GetNextPrintAsync(CancellationToken cancellationToken = default);
    Task EnqueueOrderAsync(int orderId);
}
