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
        public int id;
        public TcpClient tcpClient;
        public NetworkStream networkStream;
        public Action<GameClient> clientDisconnected;
        public Action<GameClient, string> dataReceived;

        private Pipe pipe;
        private List<byte> sendDataBytes;

        public bool isRunning { get; private set; }

        public string ip => tcpClient.getIp();

        public GameClient(TcpClient tc, int dataBufferSize = 10240)
        {
            tcpClient = tc;

            sendDataBytes = new List<byte>(dataBufferSize);
            networkStream = tcpClient.GetStream();
            pipe = new Pipe();
        }

        public async Task<(bool hasResult, string data)> read(bool isLoop = false)
        {
            try
            {
                do
                {
                    var data = await NetworkHelper.read(networkStream, pipe, false);

                    return (true, data);
                }
                while (isLoop);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());

                disconnect();

                clientDisconnected?.Invoke(this);
            }

            return (false, null);
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
            pipe.Reader.Complete();
            pipe.Writer.Complete();

            networkStream.Close();
            tcpClient.Close();
        }
    }
}
