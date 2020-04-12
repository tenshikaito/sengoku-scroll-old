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

        public MasterData masterData = new MasterData();

        public GameData gameData = new GameData();

        public MainTileMap mainTileMap;

        public MainTileMapData mainTileMapData;

        public DetailTileMap detailTileMap;

        public DetailTileMapData detailTileMapData;

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
