using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameWorld : GameWorldMap
    {
        public IncreasedIdDictionary<GamePlayer> player = new IncreasedIdDictionary<GamePlayer>();

        public GameOption gameOption = new GameOption();

        public GameData gameData;

        public GameWorldProcessor gameWorldProcessor { get; private set; }
        public GameResourceManager gameResourceProcessor { get; private set; }

        public GameWorld(string name) : base(name)
        {
        }

        public void init()
        {
            gameWorldProcessor = new GameWorldProcessor(name);
            gameResourceProcessor = new GameResourceManager(resourcePackageName);
        }
    }
}
