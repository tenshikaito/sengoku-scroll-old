using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Network
{
    public class JoinGameRequestData : BaseData
    {
        public string playerCode;
        public string playerName;
    }
}
