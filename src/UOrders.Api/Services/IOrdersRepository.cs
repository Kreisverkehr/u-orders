using UOrders.DTOModel.V1;
using UOrders.EFModel;

namespace UOrders.Api.Services;

public interface IOrdersRepository
{
    #region Public Methods

    OrderDTO? GetOrder(int orderId, string lang = "");
    public decimal GetOrderTotalPrice(Order order);
    public decimal GetOrderTotalPrice(int orderId);

    #endregion Public Methods
}