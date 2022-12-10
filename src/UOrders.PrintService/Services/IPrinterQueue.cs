using UOrders.DTOModel.V1;

namespace UOrders.PrintService.Services;

public interface IPrinterQueue
{
    #region Public Methods

    Task EnqueueOrderAsync(OrderDTO order, CancellationToken cancellationToken = default);

    Task<OrderDTO> GetNextOrderAsync(CancellationToken cancellationToken = default);

    #endregion Public Methods
}