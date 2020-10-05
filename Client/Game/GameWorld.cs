using Client.Model;
using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Game
{
    public class GameWorld
    {
        public GameWorldData gameWorldData;

        public Player currentPlayer;

        public Camera camera;

        public Cache cache = new Cache();

        public GameWorldManager gameWorldManager { get; private set; }
        public GameResourceManager gameResourceManager { get; private set; }

        public GameWorld(string name)
        {
            gameWorldManager = new GameWorldManager(name);
        }

        public void init(GameWorldData gwd)
        {
            gameWorldData = gwd;

            gameResourceManager = new GameResourceManager(gwd.resourcePackageName);
        }
    }
}
