using Quartz;
using UOrders.EFModel;
using UOrders.Service.Extensions;

namespace UOrders.Service.Jobs;

/// <summary>
/// This is a rather hacky solution to prevent orphanded texts piling up
/// </summary>
[DisallowConcurrentExecution]
internal class DeleteUnusedTextsJob : IJob
{
    #region Private Fields

    private readonly UOrdersDbContext _uOrdersDb;

    #endregion Private Fields

    #region Public Constructors

    public DeleteUnusedTextsJob(UOrdersDbContext uOrdersDb)
    {
        _uOrdersDb = uOrdersDb;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        _uOrdersDb.Texts.RemoveRange(_uOrdersDb.Texts.DynamicFilterTarget(texts =>
            from text in texts
            where text.MenuCategoryTitle == null
               && text.MenuCategoryDescription == null
               && text.MenuItemTitle == null
               && text.MenuItemDescription == null
               && text.MenuItemOptionName == null
               && text.MenuItemOptionDescription == null
               && text.MenuItemOptionValueName == null
            select text));

        await _uOrdersDb.SaveChangesAsync();
    }

    #endregion Public Methods
}