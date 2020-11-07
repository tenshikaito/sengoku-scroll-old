using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameWorld
    {
        public IncreasedIdDictionary<GamePlayer> player = new IncreasedIdDictionary<GamePlayer>().init();

        public GameWorldData gameWorldData;

        public string name { get; private set; }

        public GameWorldManager gameWorldManager { get; private set; }
        public GameResourceManager gameResourceManager { get; private set; }

        public GameWorld(string name)
        {
            this.name = name;

            gameWorldManager = new GameWorldManager(name);
        }

        public void init(GameWorldData gm)
        {
            gameWorldData = gm;

            gameResourceManager = new GameResourceManager(gameWorldData.resourcePackageName);
        }
    }
}
