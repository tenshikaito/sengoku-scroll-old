using Library;
using Server.Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Server.Game
{
    public class GameData
    {
        public IncreasedIdDictionary<Force> force;
        public IncreasedIdDictionary<Province> province;
        public IncreasedIdDictionary<Stronghold> stronghold;

        public IncreasedIdDictionary<Player> player;
    }
}
