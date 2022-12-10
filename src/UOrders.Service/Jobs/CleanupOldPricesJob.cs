using Quartz;
using UOrders.EFModel;
using UOrders.Service.Extensions;

namespace UOrders.Service.Jobs;

internal class CleanupOldPricesJob : IJob
{
    #region Private Fields

    private readonly UOrdersDbContext _uOrdersDb;

    #endregion Private Fields

    #region Public Constructors

    public CleanupOldPricesJob(UOrdersDbContext uOrdersDb)
    {
        _uOrdersDb = uOrdersDb;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        IEnumerable<Price> GetUnusedPrices(IEnumerable<MenuItem> items)
        {
            var pastItemPrices =
                from item in items
                from price in item.Prices
                where price.ValidTo != null
                   && price.ValidTo < DateTimeOffset.Now
                   && !_uOrdersDb.Orders.Where(o => o.OrderedOn > price.ValidFrom && o.OrderedOn < price.ValidTo).Any()
                select price;

            var pastOptionPrices =
                from item in items
                from option in item.Options
                from val in option.Values
                from price in val.PriceChangeToBase
                where price.ValidTo != null
                   && price.ValidTo < DateTimeOffset.Now
                   && !_uOrdersDb.Orders.Where(o => o.OrderedOn > price.ValidFrom && o.OrderedOn < price.ValidTo).Any()
                select price;

            return pastItemPrices.Union(pastOptionPrices);
        }

        _uOrdersDb.RemoveRange(_uOrdersDb.MenuItems.DynamicFilterTarget(GetUnusedPrices));

        await _uOrdersDb.SaveChangesAsync();
    }

    #endregion Public Methods
}