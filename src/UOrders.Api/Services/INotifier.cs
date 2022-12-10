namespace UOrders.Api.Services;

public interface INotifier
{
    #region Public Methods

    bool CanBeNotified(int orderId);

    void Notify(int orderId);

    #endregion Public Methods
}