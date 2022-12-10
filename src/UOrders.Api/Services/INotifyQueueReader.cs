namespace UOrders.Api.Services;

public interface INotifyQueueReader
{
    #region Public Methods

    Task<int> GetNextOrderAsync(CancellationToken cancellationToken = default);

    #endregion Public Methods
}