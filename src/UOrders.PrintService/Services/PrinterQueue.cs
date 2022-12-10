using System.Threading.Channels;
using UOrders.DTOModel.V1;

namespace UOrders.PrintService.Services;

internal sealed class PrinterQueue : IPrinterQueue
{
    #region Private Fields

    private readonly Channel<OrderDTO> orderChannel = Channel.CreateUnbounded<OrderDTO>();

    #endregion Private Fields

    #region Public Methods

    public async Task EnqueueOrderAsync(OrderDTO order, CancellationToken cancellationToken = default)
    {
        if (await orderChannel.Writer.WaitToWriteAsync(cancellationToken))
        {
            await orderChannel.Writer.WriteAsync(order);
        }
    }

    public async Task<OrderDTO> GetNextOrderAsync(CancellationToken cancellationToken = default)
    {
        if (await orderChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            return await orderChannel.Reader.ReadAsync(cancellationToken);
        }

        throw new InvalidOperationException();
    }

    #endregion Public Methods
}