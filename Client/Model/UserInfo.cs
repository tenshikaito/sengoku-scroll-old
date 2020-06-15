using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class UserInfo
    {
        public string name;
        public string code;
        public int scrollSpeed = 10;
        public List<ServerInfo> servers;
    }
}
