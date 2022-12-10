namespace UOrders.Api.Services;

public interface IPrinter
{
    Task PrintOrderAsync(int orderId, CancellationToken cancellationToken = default);
}
