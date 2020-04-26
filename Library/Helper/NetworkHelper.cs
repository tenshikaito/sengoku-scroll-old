
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public static class NetworkHelper
    {
        public static string getIp(this TcpClient tc) => tc.Client.RemoteEndPoint.ToString();
    }
}
