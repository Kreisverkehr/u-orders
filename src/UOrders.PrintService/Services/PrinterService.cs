using ESCPOS_NET;

namespace UOrders.PrintService.Services;

public sealed class PrinterService : BackgroundService
{
    #region Private Fields

    private readonly ILogger<PrinterService> _logger;
    private readonly IPrinter _printer;
    private readonly IPrinterQueue _printerQueue;
    private readonly IPrintFormatter _printFormatter;

    #endregion Private Fields

    #region Public Constructors

    public PrinterService(ILogger<PrinterService> logger, IPrinterQueue printerQueue, IPrintFormatter printFormatter, IPrinter printer)
    {
        _logger = logger;
        _printerQueue = printerQueue;
        _printFormatter = printFormatter;
        _printer = printer;
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var order = await _printerQueue.GetNextOrderAsync(stoppingToken);
            var orderFormated = _printFormatter.FormatOrder(order);
            _printer.Write(orderFormated);
        }
    }

    #endregion Protected Methods
}