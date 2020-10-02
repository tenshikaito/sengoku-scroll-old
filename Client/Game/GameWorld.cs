using Client.Model;
using Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Game
{
    public class GameWorld : GameWorldMap
    {
        public GameOption gameOption = new GameOption();

        public GameData gameData;

        public Player currentPlayer;

        public Camera camera;

        public Cache cache = new Cache();

        public GameWorldProcessor gameWorldProcessor { get; private set; }
        public GameResourceManager gameResourceManager { get; private set; }

        public GameWorld(string name) : base(name)
        {
        }

        public void init()
        {
            gameWorldProcessor = new GameWorldProcessor(name);
            gameResourceManager = new GameResourceManager(resourcePackageName);
        }
    }
}
