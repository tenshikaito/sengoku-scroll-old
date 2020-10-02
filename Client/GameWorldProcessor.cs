using Client.Game;
using Client.Helper;
using Library;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static Library.Helper.FileHelper;

namespace Client
{
    public class GameWorldProcessor : Library.GameWorldManager
    {
        public Part map { get; }

        public Part game { get; }

        public GameWorldProcessor(string gameWorldName)
        {
            map = new Part(MapDirName, gameWorldName);
            game = new Part(GameDirName, gameWorldName);
        }

        public new class Part : Library.GameWorldManager.Part
        {
            public Part(string dirName, string gameWorldName) : base(dirName, gameWorldName)
            {
            }

            public async ValueTask<bool> createDirectory(int width, int height)
            {
                var path = fullPath;

                if (Directory.Exists(path)) return false;

                Directory.CreateDirectory(path);

                var md = ExampleHelper.getMasterData();

                var tm = ExampleHelper.getTileMap(width, height);

                var gd = ExampleHelper.getGameData();

                await saveMasterData(new GameWorldMap(gameWorldName)
                {
                    gameDate = new GameDate().addDay(60),
                    masterData = md,
                    tileMap = tm,
                });

                await saveGameData(gd);

                return true;
            }

            public async Task saveGameData(GameData gd)
            {
                var path = fullPath;

                await save(path + GameDateName, gd);
            }

        }
    }
}
