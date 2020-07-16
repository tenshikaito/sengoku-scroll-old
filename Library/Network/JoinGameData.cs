using Library.Helper;
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
    }

    public class JoinGameResponseData : BaseData
    {
        public GameWorldMap gameWorldMap;
    }
}
