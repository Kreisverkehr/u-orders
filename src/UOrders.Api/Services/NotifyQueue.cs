using System.Threading.Channels;

namespace UOrders.Api.Services;

internal sealed class NotifyQueue : INotifyQueue
{
    #region Private Fields

    private readonly Channel<int> channel = Channel.CreateUnbounded<int>();

    #endregion Private Fields

    #region Public Methods

    public async Task EnqueueOrderAsync(int orderId, CancellationToken cancellationToken = default) =>
        await channel.Writer.WriteAsync(orderId, cancellationToken);

    public async Task<int> GetNextOrderAsync(CancellationToken cancellationToken = default) =>
        await channel.Reader.ReadAsync(cancellationToken);

    #endregion Public Methods
}