using Microsoft.Extensions.Options;
using UOrders.Api.Controllers.V1;
using UOrders.Api.Options;

namespace UOrders.Api.Services;

internal sealed class PrintWorker : BackgroundService
{
    private readonly ILogger<PrintWorker> _logger;
    private readonly IPrintQueue _printQueue;
    private readonly IServiceProvider _serviceProvider;

    public PrintWorker(ILogger<PrintWorker> logger, IPrintQueue printQueue, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _printQueue = printQueue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting print worker");
        while (!stoppingToken.IsCancellationRequested)
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderId = await _printQueue.GetNextPrintAsync(stoppingToken);
                _logger.LogInformation("Printing order #{oderId}", orderId);
                var printer = scope.ServiceProvider.GetRequiredService<IPrinter>();
                await printer.PrintOrderAsync(orderId, stoppingToken);
            }
        _logger.LogInformation("Stopping print worker");
    }
}
