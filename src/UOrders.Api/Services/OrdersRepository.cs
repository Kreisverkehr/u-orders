using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UOrders.DTOModel.V1;
using UOrders.EFModel;

namespace UOrders.Api.Services;

public sealed class OrdersRepository : IOrdersRepository
{
    #region Private Fields

    private readonly UOrdersDbContext _dbContext;
    private readonly ILogger<OrdersRepository> _logger;
    private readonly IMapper _mapper;

    #endregion Private Fields

    #region Public Constructors

    public OrdersRepository(ILogger<OrdersRepository> logger, UOrdersDbContext dbContext, IMapper mapper)
    {
        this._logger = logger;
        this._dbContext = dbContext;
        this._mapper = mapper;
    }

    #endregion Public Constructors

    #region Public Methods

    public OrderDTO? GetOrder(int orderId, string lang = "")
    {
        if (!_dbContext.Orders.Any()) return null;

        var order = _mapper.ProjectTo<OrderDTO>(_dbContext.Orders, new { lang }).Where(o => o.ID == orderId).First();
        if (order == null) return null;

        foreach (var item in order.OrderedItems)
        {
            var dbMenuItem = _dbContext.MenuItems.Find(item.MenuItemID);
            if (dbMenuItem == null) return null;

            item.UnitPrice = dbMenuItem
                .Prices
                .Where(p => p.ValidFrom <= order.OrderedOn && (!p.ValidTo.HasValue || p.ValidTo.Value > order.OrderedOn))
                .Select(p => p.Value)
                .First();

            item.UnitPrice += dbMenuItem.Options.SelectMany(o => o.Values)
                .Where(v => item.CheckedOptions.Select(ov => ov.OptionValueID).Contains(v.ID))
                .SelectMany(v => v.PriceChangeToBase)
                .Where(p => p.ValidFrom <= order.OrderedOn && (!p.ValidTo.HasValue || p.ValidTo.Value > order.OrderedOn))
                .Select(p => p.Value)
                .Sum();

            item.TotalPrice = item.UnitPrice * item.Count;
        }

        order.TotalPrice = order.OrderedItems.Select(i => i.TotalPrice).Sum();

        return order;
    }

    public decimal GetOrderTotalPrice(int orderId)
    {
        var basePriceComponents =
            from dbOrder in _dbContext.Orders
            where dbOrder.ID == orderId
            from item in dbOrder.OrderedItems
            from price in item.MenuItem.Prices
            where price.ValidFrom <= dbOrder.OrderedOn
            where !price.ValidTo.HasValue || price.ValidTo > dbOrder.OrderedOn
            select price.Value * item.Count;
        var optionPriceComponents =
            from dbOrder in _dbContext.Orders
            where dbOrder.ID == orderId
            from item in dbOrder.OrderedItems
            from option in item.CheckedOptions
            from price in option.OptionValue.PriceChangeToBase
            where price.ValidFrom <= dbOrder.OrderedOn
            where !price.ValidTo.HasValue || price.ValidTo > dbOrder.OrderedOn
            select price.Value * item.Count;

        return basePriceComponents.Union(optionPriceComponents).Sum();
    }

    public decimal GetOrderTotalPrice(Order order) =>
        GetOrderTotalPrice(order.ID);

    #endregion Public Methods
}