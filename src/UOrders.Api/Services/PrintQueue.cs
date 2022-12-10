using System.Threading.Channels;

namespace UOrders.Api.Services;

internal sealed class PrintQueue : IPrintQueue
{
    private readonly Channel<int> channel = Channel.CreateUnbounded<int>();

    public async Task EnqueueOrderAsync(int orderId)
    {
        await channel.Writer.WriteAsync(orderId);
    }

    public async Task<int> GetNextPrintAsync(CancellationToken cancellationToken = default)
    {
        if (await channel.Reader.WaitToReadAsync(cancellationToken))
            return await channel.Reader.ReadAsync(cancellationToken);

        throw new InvalidOperationException("could not get new PrintJob");
    }
}
