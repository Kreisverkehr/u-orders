using Quartz;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UOrders.Service.Jobs;

[DisallowConcurrentExecution]
internal class BroadcastDbReadyEventJob : IJob
{
    #region Private Fields

    private readonly UdpClient _udpClient = new();

    #endregion Private Fields

    #region Public Constructors

    public BroadcastDbReadyEventJob()
    {
        _udpClient.EnableBroadcast = true;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        var msg = Encoding.UTF8.GetBytes("DB-READY");
        await _udpClient.SendAsync(msg, msg.Length, new(IPAddress.Broadcast, 9876));
    }

    #endregion Public Methods
}