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
using Region = Library.Model.Region;
using static Library.Helper.FileHelper;

namespace Library
{
    public class GameWorldManager
    {
        protected const string MapDirName = "map";
        protected const string GameDirName = "game";

        protected const string MainMapName = "/main_map.dat";
        protected const string DetailMapName = "/detail_map.dat";
        protected const string DetailMapDataName = "/detail_map_data.dat";
        protected const string MasterDataName = "/master_data.dat";
        protected const string GameDateName = "/game_date.dat";
        protected const string GameDataName = "/game_data.dat";

        public static List<string> getMapList() => getFileList(MapDirName);

        public static List<string> getGameList() => getFileList(GameDirName);

        protected static List<string> getFileList(string dirName)
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{dirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateDirectories(path).Select(o => new DirectoryInfo(o).Name).ToList();
        }

        public static void publishMap(string name)
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{GameDirName}/{name}";
            var mapPath = $"{currentDirPath}/{MapDirName}/{name}";

            if (Directory.Exists(path)) Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            Directory.EnumerateFiles(mapPath).Select(o => new FileInfo(o)).ToList().ForEach(o => o.CopyTo($"{path}/{o.Name}"));
        }

        public class Part
        {
            public string dirName { get; }
            public string gameWorldName { get; }

            public string path => $"{dirName}/{gameWorldName}";

            public string fullPath => $"{Directory.GetCurrentDirectory()}/{path}";

            public Part(string dirName, string gameWorldName)
            {
                this.dirName = dirName;
                this.gameWorldName = gameWorldName;
            }

            public string getFilePath(string fileName, bool isAbsolute = false)
                => isAbsolute ? $"{fullPath}/{fileName}" : $"{path}/{fileName}";

            public void deleteDirectory() => Directory.Delete(fullPath, true);

            public async Task<T> loadMasterData<T>(T gw) where T : GameWorldMap
            {
                var path = fullPath;

                gw.tileMap = await load<TileMap>(path + MainMapName);
                gw.gameDate = await load<GameDate>(path + GameDateName);
                gw.masterData = await load<MasterData>(path + MasterDataName);

                return gw;
            }

            public async Task saveMasterData(GameWorldMap gw)
            {
                var path = fullPath;

                await save(path + MainMapName, gw.tileMap);
                await save(path + GameDateName, gw.gameDate);
                await save(path + MasterDataName, gw.masterData);
            }
        }
    }
}
