using UOrders.DTOModel.V1;

namespace UOrders.PrintService.Services;

public interface IPrintFormatter
{
    #region Public Methods

    byte[][] FormatOrder(OrderDTO order);

    #endregion Public Methods
}