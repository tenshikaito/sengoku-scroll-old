
using Library.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public static class NetworkHelper
    {
        public static readonly Encoding encoding = Encoding.UTF8;

        public static string getIp(this TcpClient tc) => tc.Client.RemoteEndPoint.ToString();

        public static GameClient getGameClient() => new GameClient(new TcpClient(), encoding);

        public static async Task write(this GameClient gc, string name, BaseData bd) => await gc.write(bd.toCommandString(name));

        public static async Task send(this GameClient gc, string ip, int port, string data, Action<GameClient, string> onDataReceived)
        {
            if (!gc.tcpClient.Connected) await gc.tcpClient.ConnectAsync(ip, port);

            if (!gc.tcpClient.Connected) return;

            gc.dataReceived = onDataReceived;

            gc.start();

            await gc.write(data);
        }
    }
}
