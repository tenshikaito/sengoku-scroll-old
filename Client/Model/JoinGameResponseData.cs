using Client.Game;
using Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Model
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
