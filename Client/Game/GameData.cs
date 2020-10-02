using Client.Game.Model;
using Client.Model;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Client.Game
{
    public class GameData
    {
        public IncreasedIdDictionary<Force> force;
        public IncreasedIdDictionary<Province> province;
        public IncreasedIdDictionary<Stronghold> stronghold;

        public IncreasedIdDictionary<Player> player;
    }
}
