#define TEST

using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Library.Helper.FileHelper;

namespace Library
{
    public class GameWorldManager
    {
        protected const string MapDirName = "map";
        protected const string GameDirName = "game";
        protected const string Extension = "ssm";

        public Part map { get; }

        public Part game { get; }

        public GameWorldManager(string gameWorldName)
        {
            map = new Part(MapDirName, gameWorldName);
            game = new Part(GameDirName, gameWorldName);
        }

        public static List<string> getMapList() => getFileList(MapDirName);

        public static List<string> getGameList() => getFileList(GameDirName);

        protected static List<string> getFileList(string dirName)
        {
            var path = $"{Directory.GetCurrentDirectory()}/{dirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateFiles(path, $"*.{Extension}").Select(o => Path.GetFileNameWithoutExtension(o)).ToList();
        }

        public static void publishMap(string name)
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{GameDirName}/{name}.{Extension}";
            var mapPath = $"{currentDirPath}/{MapDirName}/{name}.{Extension}";

            if (File.Exists(path)) File.Delete(path);

            new FileInfo(mapPath).CopyTo(path);
        }

        public class Part
        {
            public string dirName { get; }
            public string gameWorldName { get; }

            public string path => $"{dirName}/{gameWorldName}.{Extension}";

            public string fullPath => $"{Directory.GetCurrentDirectory()}/{path}";

            public Part(string dirName, string gameWorldName)
            {
                this.dirName = dirName;
                this.gameWorldName = gameWorldName;
            }

            public void delete() => File.Delete(fullPath);

            public async Task<GameWorldData> loadGameWorldData()
            {
                return await load<GameWorldData>(fullPath);
            }

            public async Task saveGameWorldData(GameWorldData gwd)
            {
                await save(fullPath, gwd);
            }

            public async ValueTask<bool> create(int width, int height)
            {
                if (File.Exists(path)) return false;

                var md = ExampleHelper.getMasterData();

                var tm = ExampleHelper.getTileMap(width, height);

                var gd = ExampleHelper.getGameData();

                await saveGameWorldData(new GameWorldData(gameWorldName)
                {
                    gameDate = ExampleHelper.getGameDate(),
                    tileMap = tm,
                    masterData = md,
                    gameData = gd
                });

                return true;
            }
        }
    }
}
