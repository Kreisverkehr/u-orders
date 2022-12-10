namespace UOrders.Api.Services;

public class NotificationWorker : BackgroundService
{
    #region Private Fields

    private readonly INotifyQueueReader _queueReader;
    private readonly IServiceProvider _serviceProvider;

    #endregion Private Fields

    #region Public Constructors

    public NotificationWorker(INotifyQueueReader queueReader, IServiceProvider serviceProvider)
    {
        _queueReader = queueReader;
        this._serviceProvider = serviceProvider;
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var order = await _queueReader.GetNextOrderAsync();
            using var scope = _serviceProvider.CreateScope();
            foreach (var notifier in scope.ServiceProvider.GetServices<INotifier>().Where(n => n.CanBeNotified(order)))
                notifier.Notify(order);
        }
    }

    #endregion Protected Methods
}