namespace UOrders.Api.Services;

public interface INotifyQueueWriter
{
    #region Public Methods

    Task EnqueueOrderAsync(int orderId, CancellationToken cancellationToken = default);

    #endregion Public Methods
}