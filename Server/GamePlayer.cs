﻿using Library;
using Library.Model;
using Library.Network;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GamePlayer
    {
        public int id;

        public string code;

        public string playerName;

        public Player player;

        public GameClient gameClient;
    }
}
