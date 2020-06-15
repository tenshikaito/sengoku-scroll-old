﻿#define TEST

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
    public class GameWorldProcessor
    {
        private const string MapDirName = "map";
        private const string GameDirName = "game";

        private const string MainMapName = "/main_map.dat";
        private const string DetailMapName = "/detail_map.dat";
        private const string DetailMapDataName = "/detail_map_data.dat";
        private const string MasterDataName = "/master_data.dat";
        private const string GameDateName = "/game_date.dat";
        private const string GameDataName = "/game_data.dat";

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

                var gd = ExampleHelper.getGameData();

                saveMasterData(new GameWorldMap(gameWorldName)
                {
                    gameDate = new GameDate().addDay(60),
                    masterData = md,
                    gameData = gd,
                    mainTileMap = tm,
                });

                return true;
            }

            public string getFilePath(string fileName, bool isAbsolute = false)
                => isAbsolute ? $"{fullPath}/{fileName}" : $"{path}/{fileName}";

            public void deleteDirectory() => Directory.Delete(fullPath, true);

            public T loadMasterData<T>(T gw) where T : GameWorldMap
            {
                var path = fullPath;

                gw.mainTileMap = load<MainTileMap>(path + MainMapName);
                gw.gameDate = load<GameDate>(path + GameDateName);
                gw.masterData = load<MasterData>(path + MasterDataName);

                return gw;
            }

            public void saveMasterData(GameWorldMap gw)
            {
                var path = fullPath;

                save(path + MainMapName, gw.mainTileMap);
                save(path + GameDateName, gw.gameDate);
                save(path + MasterDataName, gw.masterData);
            }

            public T loadGameData<T>(T gw) where T : GameWorldMap
            {
                loadMasterData(gw);

                gw.gameData = load<GameData>(fullPath + MainMapName);

                return gw;
            }

            public DetailTileMap loadDetailTileMap(int id, int width, int height)
            {
                var path = fullPath + string.Format(DetailMapDataName, id);

                if (!File.Exists(path)) return new DetailTileMap(new TileMap.Size(height, width)) { tiles = new DetailMapTile[width * height] };
                
                return load<DetailTileMap>(path);
            }

            public void saveDetailTileMap(int id, DetailTileMap tm)
            {
                var path = fullPath + string.Format(DetailMapDataName, id);

                save(path, tm);
            }
        }
    }
}
