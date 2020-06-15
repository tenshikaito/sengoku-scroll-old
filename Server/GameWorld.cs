using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameWorld : GameWorldMap
    {
        public IncreasedIdDictionary<GamePlayer> player = new IncreasedIdDictionary<GamePlayer>();

        public GameOption gameOption = new GameOption();

        public GameWorld(string name) : base(name)
        {
        }
    }
}
