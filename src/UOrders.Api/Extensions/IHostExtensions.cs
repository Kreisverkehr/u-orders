using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UOrders.Api.Extensions;

public static class IHostExtensions
{
    #region Public Methods

    public static void WaitForScheduler<HostType>(this HostType host, TimeSpan? waitTime = null, int retries = 0, Action? dbUnreachableCallback = null) where HostType : IHost
    {
        if (waitTime == null) waitTime = TimeSpan.Zero;

        var logger = host.Services.GetRequiredService<ILogger<HostType>>();

        logger.LogInformation("Waiting for scheduler");

        using UdpClient udpClient = new();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 9876));

        bool ready = false;
        var from = new IPEndPoint(0, 0);
        while (!ready)
        {
            var recvBuffer = udpClient.Receive(ref from);
            ready = Encoding.UTF8.GetString(recvBuffer) == "DB-READY";
        }
        logger.LogInformation("Scheduler now up. Starting API server.");
    }

    #endregion Public Methods
}