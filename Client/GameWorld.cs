using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class GameWorld
    {
        public string name { get; }

        public GameOption gameOption = new GameOption();

        public GameWorldMasterData gameWorldMasterData = new GameWorldMasterData();

        public GameWorldGameData gameWorldGameData = new GameWorldGameData();

        public GameWorldOuterMapData gameOuterMapData;

        public GameWorldInnerMapData gameInnerMapData;

        public Character currentCharacter;

        public Camera camera;

        public Cache cache = new Cache();

        private GameWorldProcessor gameWorldProcessor;

        public GameWorld(string name)
        {
            this.name = name;
        }

        public GameWorldProcessor getGameWorldProcessor() => gameWorldProcessor = gameWorldProcessor ?? new GameWorldProcessor(name);
    }
}
