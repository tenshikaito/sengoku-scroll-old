
using Library.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static GameClient getGameClient(TcpClient tc = null, int dataBufferSize = 10240, int socketBufferSize = 4096)
            => new GameClient(tc ?? new TcpClient(), dataBufferSize, socketBufferSize);

        public static async Task write(this GameClient gc, string name, BaseData bd) => await gc.write(bd.toCommandString(name));

        public static async Task send(this GameClient gc, string ip, int port, string data, Action<GameClient, string> onDataReceived)
        {
            if (!gc.tcpClient.Connected) await gc.connect(ip, port);

            gc.dataReceived = onDataReceived;

            gc.start();

            await gc.write(data);
        }

        public static (bool hasResult, string data) splitDataStream(List<byte> receiveDataBytes)
        {
            var list = receiveDataBytes;

            if (list.Count < 4) return (false, null);

            var length = BitConverter.ToInt32(list.Take(4).ToArray(), 0);

            if (list.Count - 4 < length) return (false, null);

            list.RemoveRange(0, 4);

            var data = list.Take(length).ToArray();

            list.RemoveRange(0, length);

            var s = encoding.GetString(data);

            return (true, s);
        }

        public static byte[] combineDataStream(List<byte> sendDataBytes, string data)
        {
            var buffer = BitConverter.GetBytes(data.Length);

            sendDataBytes.AddRange(buffer);

            buffer = encoding.GetBytes(data);

            sendDataBytes.AddRange(buffer);

            return sendDataBytes.ToArray();
        }
    }
}
