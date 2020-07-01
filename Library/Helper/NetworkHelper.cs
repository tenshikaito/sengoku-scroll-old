
using Library.Network;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public static class NetworkHelper
    {
        public const int dataLengthByteLength = 4;
        public const int dataBufferSize = 10240;

        public static readonly Encoding encoding = Encoding.UTF8;

        public static string getIp(this TcpClient tc) => tc.Client.RemoteEndPoint.ToString();

        public static GameClient getGameClient(TcpClient tc = null, int dataBufferSize = dataBufferSize)
            => new GameClient(tc ?? new TcpClient(), dataBufferSize);

        public static async Task write(this GameClient gc, string name, BaseData bd) => await gc.write(bd.toCommandString(name));

        public static (bool hasResult, string data) splitDataStream(List<byte> receiveDataBytes)
        {
            var list = receiveDataBytes;

            if (list.Count < dataLengthByteLength) return (false, null);

            var length = BitConverter.ToInt32(list.Take(dataLengthByteLength).ToArray(), 0);

            if (list.Count - dataLengthByteLength < length) return (false, null);

            list.RemoveRange(0, dataLengthByteLength);

            var data = list.Take(length).ToArray();

            list.RemoveRange(0, length);

            var s = encoding.GetString(data);

            return (true, s);
        }

        public static (bool hasResult, string data) splitDataStream(ref ReadOnlySequence<byte> buffer)
        {
            if (buffer.Length < dataLengthByteLength) return (false, null);

            var length = BitConverter.ToInt32(buffer.Slice(0, dataLengthByteLength).ToArray(), 0);

            if (buffer.Length - dataLengthByteLength < length) return (false, null);

            var data = buffer.Slice(dataLengthByteLength, length).ToArray();

            var s = encoding.GetString(data);

            buffer = buffer.Slice(dataLengthByteLength + length);

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

        public static async Task<string> fetch(string ip, int port, string data)
        {
            using (var tc = new TcpClient())
            {
                await tc.ConnectAsync(ip, port);

                using (var ns = tc.GetStream())
                {
                    var bytes = combineDataStream(new List<byte>(), data);

                    await ns.WriteAsync(bytes);

                    await ns.FlushAsync();

                    var p = new Pipe();
                    var w = p.Writer;
                    var r = p.Reader;

                    while (true)
                    {
                        var m = w.GetMemory(dataBufferSize);

                        var length = await ns.ReadAsync(m);

                        if (length == 0) return null;

                        w.Advance(length);

                        await w.FlushAsync();

                        var rr = await r.ReadAsync();
                        var buffer = rr.Buffer;

                        var rrr = splitDataStream(ref buffer);

                        if (rrr.hasResult)
                        {
                            await w.CompleteAsync();
                            await r.CompleteAsync();
                            return rrr.data;
                        }
                    }
                }
            }
        }
    }
}
