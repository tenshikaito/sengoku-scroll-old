using Library;
using Server.Game;
using Server.Game.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Model
{
    public class JoinGameResponseData
    {
        public GameWorldMapModel gameWorldMap;
        public Player player;

        public class GameWorldMapModel : GameWorldMap
        {
            public GameData gameData;

            public GameWorldMapModel(string name) : base(name)
            {
            }
        }
    }
}
