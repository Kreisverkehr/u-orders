using Microsoft.EntityFrameworkCore;
using Quartz;
using UOrders.EFModel;
using UOrders.Service.Extensions;

namespace UOrders.Service.Jobs;

internal class CleanupOldMenuObjects : IJob
{
    #region Private Fields

    private readonly UOrdersDbContext _uOrdersDb;

    #endregion Private Fields

    #region Public Constructors

    public CleanupOldMenuObjects(UOrdersDbContext uOrdersDb)
    {
        _uOrdersDb = uOrdersDb;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        // Load dependant data. EfCore needs to know these in order for client side cascade to work.
        // Server side cascade won't work because of the complex relationships
        _uOrdersDb.Prices.ToList();
        _uOrdersDb.Texts.ToList();

        _uOrdersDb.Categories.RemoveRange(_uOrdersDb.Categories.DynamicFilterTarget(categories =>
            from cat in categories
            where cat.ToBeRemoved
                && !cat.Items.Intersect(
                        _uOrdersDb.Orders
                        .SelectMany(o => o.OrderedItems)
                        .Select(oi => oi.MenuItem)
                    ).Any()
            select cat
        ));

        _uOrdersDb.MenuItems.RemoveRange(_uOrdersDb.MenuItems.DynamicFilterTarget(items =>
            from item in items
            where item.ToBeRemoved
               && !_uOrdersDb.Orders
                    .SelectMany(o => o.OrderedItems)
                    .Where(oi => oi.MenuItem == item)
                    .Any()
            select item
        ));

        _uOrdersDb.MenuItemOptions.RemoveRange(_uOrdersDb.MenuItems.DynamicFilterTarget(items =>
            from item in items
            from option in item.Options
            where option.ToBeRemoved
               && !option.Values.Intersect(
                       _uOrdersDb.Orders
                       .SelectMany(o => o.OrderedItems)
                       .SelectMany(oi => oi.CheckedOptions)
                       .Select(co => co.OptionValue)
                   ).Any()
            select option
        ));

        _uOrdersDb.MenuItemOptionValues.RemoveRange(_uOrdersDb.MenuItems.DynamicFilterTarget(items =>
            from item in items
            from option in item.Options
            from val in option.Values
            where val.ToBeRemoved
               && !_uOrdersDb.Orders
                    .SelectMany(o => o.OrderedItems)
                    .SelectMany(i => i.CheckedOptions)
                    .Where(co => co.OptionValue == val)
                    .Any()
            select val
        ));

        await _uOrdersDb.SaveChangesAsync();
    }

    #endregion Public Methods

    #region Private Methods



    #endregion Private Methods
}