using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameWorld : GameWorldMap
    {
        public string name { get; }
        public string resourcePackageName { get; set; }

        public GameOption gameOption = new GameOption();

        public GameWorldProcessor gameWorldProcessor { get; private set; }
        public GameResourceProcessor gameResourceProcessor { get; private set; }

        public GameWorld(string name)
        {
            this.name = name;
        }

        public GameWorld init()
        {
            gameWorldProcessor = new GameWorldProcessor(name);
            gameResourceProcessor = new GameResourceProcessor(resourcePackageName);

            return this;
        }
    }
}
