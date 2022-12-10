using Microsoft.Extensions.Options;
using UOrders.Api.Options;

namespace UOrders.Api.Services;

internal sealed class Printer : IPrinter
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOptions<PrinterOptions> _options;
    private readonly HttpClient _httpClient;

    public Printer(IOrdersRepository ordersRepository, IOptions<PrinterOptions> options, HttpClient httpClient)
    {
        this._ordersRepository = ordersRepository;
        this._options = options;
        this._httpClient = httpClient;
    }

    public async Task PrintOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = _ordersRepository.GetOrder(orderId, _options.Value.Lang);
        try
        {
            await _httpClient.PostAsJsonAsync("Print/order", order, cancellationToken);
        }

        // prevent TaskCancelled Exception from being thrown. An order schould be reprintable.
        catch (TaskCanceledException) { }
    }
}
