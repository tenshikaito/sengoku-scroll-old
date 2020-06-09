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

namespace Library
{
    public class GameWorldProcessor
    {
        private const string MapDirName = "map";
        private const string GameDirName = "game";

        private const string MainMapName = "/main_map_.dat";
        private const string MainMapDataName = "/main_map_data.dat";
        private const string DetailMapName = "/detail_map_{0}.dat";
        private const string DetailMapDataName = "/detail_map_data_{0}.dat";
        private const string MasterDataName = "/master_data.dat";
        private const string GameDataName = "/game_data.dat";

        public static readonly Encoding encoding = Encoding.UTF8;

        public Data map { get; }

        public Data game { get; }

        public GameWorldProcessor(string gameWorldName)
        {
            map = new Data(MapDirName, gameWorldName);
            game = new Data(GameDirName, gameWorldName);
        }

        public static List<string> getMapList() => getFileList(MapDirName);

        public static List<string> getGameList() => getFileList(GameDirName);

        private static List<string> getFileList(string dirName)
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

        public sealed class Data
        {
            public string dirName { get; }
            public string gameWorldName { get; }

            public string path => $"{dirName}/{gameWorldName}";

            public string fullPath => $"{Directory.GetCurrentDirectory()}/{path}";

            public Data(string dirName, string gameWorldName)
            {
                this.dirName = dirName;
                this.gameWorldName = gameWorldName;
            }

            public bool createDirectory(int width, int height)
            {
                var path = fullPath;

                if (Directory.Exists(path)) return false;

                Directory.CreateDirectory(path);

                var md = ExampleHelper.getMasterData();

                var tm = ExampleHelper.getMainTileMap(width, height);

                var tmd = ExampleHelper.getMainTileMapData();

                var gd = ExampleHelper.getGameData();

                saveMasterData(new GameWorldMap()
                {
                    masterData = md,
                    gameData = gd,
                    mainTileMap = tm,
                    mainTileMapData = tmd
                });

                return true;
            }

            public string getFilePath(string fileName, bool isAbsolute = false)
                => isAbsolute ? $"{fullPath}/{fileName}" : $"{path}/{fileName}";

            public void deleteDirectory() => Directory.Delete(fullPath, true);

            public T loadMasterData<T>(T gw) where T : GameWorldMap
            {
                var path = fullPath;

                gw.mainTileMap = File.ReadAllText(path + MainMapName, encoding).fromJson<MainTileMap>();
                gw.mainTileMapData = File.ReadAllText(path + MainMapDataName, encoding).fromJson<MainTileMapData>();
                gw.masterData = File.ReadAllText(path + MasterDataName, encoding).fromJson<MasterData>();

                return gw;
            }

            public void saveMasterData(GameWorldMap gw)
            {
                var path = fullPath;

                File.WriteAllText(path + MainMapName, gw.mainTileMap.toJson(), encoding);
                File.WriteAllText(path + MainMapDataName, gw.mainTileMapData.toJson(), encoding);
                File.WriteAllText(path + MasterDataName, gw.masterData.toJson(), encoding);
            }

            public T loadGameData<T>(T gw) where T : GameWorldMap
            {
                loadMasterData(gw);

                gw.gameData = File.ReadAllText(fullPath + MainMapName, encoding).fromJson<GameData>();

                return gw;
            }

            public DetailTileMap loadDetailTileMap(int id, int width, int height)
            {
                var path = fullPath + string.Format(DetailMapDataName, id);

                if (!File.Exists(path)) return new DetailTileMap(new TileMap.Size(height, width)) { tiles = new DetailMapTile[width * height] };
                
                return File.ReadAllText(path, encoding).fromJson<DetailTileMap>();
            }

            public void saveDetailTileMap(int id, DetailTileMap tm)
            {
                var path = fullPath + string.Format(DetailMapDataName, id);

                File.WriteAllText(path, tm.toJson(), encoding);
            }
        }
    }
}
