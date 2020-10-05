using Library;
using Library.Helper;
using Server.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static Library.Helper.FileHelper;

namespace Server
{
    public class GameWorldManager : Library.GameWorldManager
    {
        public Part map { get; }

        public Part game { get; }

        public GameWorldManager(string gameWorldName)
        {
            map = new Part(MapDirName, gameWorldName);
            game = new Part(GameDirName, gameWorldName);
        }

        public new class Part : Library.GameWorldManager.Part
        {
            public Part(string dirName, string gameWorldName) : base(dirName, gameWorldName)
            {
            }

            public async Task<GameData> loadGameData()
            {
                return await load<GameData>(fullPath + MapName);
            }
        }
    }
}
