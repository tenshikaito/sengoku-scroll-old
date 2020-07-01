using Library.Helper;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameClient
    {
        public TcpClient tcpClient;
        public NetworkStream networkStream;
        public Action<GameClient> clientDisconnected;
        public Action<GameClient, string> dataReceived;

        private Pipe pipe;
        private int dataBufferSize;
        private List<byte> sendDataBytes;

        public bool isRunning { get; private set; }

        public string ip => tcpClient.getIp();

        public GameClient(TcpClient tc, int dataBufferSize = 10240)
        {
            tcpClient = tc;
            sendDataBytes = new List<byte>(dataBufferSize);

            this.dataBufferSize = dataBufferSize;
        }

        public void start()
        {
            isRunning = true;

            networkStream = tcpClient.GetStream();

            pipe = new Pipe();

            _ = receive();
        }

        private async Task receive()
        {
            try
            {
                var p = pipe;
                var w = p.Writer;
                var r = p.Reader;

                while (true)
                {
                    var m = w.GetMemory(dataBufferSize);

                    var length = await networkStream.ReadAsync(m);

                    if (length == 0) break;

                    w.Advance(length);

                    await w.FlushAsync();

                    var rr = await r.ReadAsync();
                    var buffer = rr.Buffer;

                    while (true)
                    {
                        var rrr = NetworkHelper.splitDataStream(ref buffer);

                        if (rrr.hasResult) dataReceived?.Invoke(this, rrr.data);
                        else break;
                    }

                    r.AdvanceTo(buffer.Start, buffer.End);
                }

                await w.CompleteAsync();
                await r.CompleteAsync();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());

                disconnect();

                clientDisconnected?.Invoke(this);
            }
        }


        public async Task write(string data)
        {
            sendDataBytes.Clear();

            var ns = networkStream;

            var r = NetworkHelper.combineDataStream(sendDataBytes, data);

            await ns.WriteAsync(r);

            await ns.FlushAsync();
        }

        public void disconnect()
        {
            networkStream.Close();
            tcpClient.Close();
        }
    }
}
