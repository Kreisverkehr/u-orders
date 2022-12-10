using Quartz;
using UOrders.EFModel;
using UOrders.Service.Extensions;

namespace UOrders.Service.Jobs;

internal class CleanupOrdersJob : IJob
{
    #region Private Fields

    private readonly UOrdersDbContext _uOrdersDb;

    #endregion Private Fields

    #region Public Constructors

    public CleanupOrdersJob(UOrdersDbContext uOrdersDb)
    {
        _uOrdersDb = uOrdersDb;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        _uOrdersDb.Orders.RemoveRange(
            _uOrdersDb.Orders.DynamicFilterTarget(orders => orders
            .OrderByDescending(o => o.OrderedOn)
            .Skip(50)
            ));

        await _uOrdersDb.SaveChangesAsync();
    }

    #endregion Public Methods
}