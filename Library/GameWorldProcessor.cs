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
        private const string MasterDataName = "/data.dat";

        public static readonly Encoding encoding = Encoding.UTF8;

        private string gameWorldName;
        private string gameWorldMapPath;
        private string gameWorldMapFullPath;
        private string gameWorldGamePath;
        private string gameWorldGameFullPath;

        public GameWorldProcessor(string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            gameWorldMapPath = $"{MapDirName}/{gameWorldName}";
            gameWorldMapFullPath = $"{Directory.GetCurrentDirectory()}/{gameWorldMapPath}";
            gameWorldGamePath = $"{GameDirName}/{gameWorldName}";
            gameWorldGameFullPath = $"{Directory.GetCurrentDirectory()}/{gameWorldGamePath}";
        }

        public static List<string> getMapList()
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{MapDirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateDirectories(path).Select(o => new DirectoryInfo(o).Name).ToList();
        }

        public static List<string> getGameList()
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{GameDirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateDirectories(path).Select(o => new DirectoryInfo(o).Name).ToList();
        }

        public bool createMapDirectory(int width, int height)
        {
            if (Directory.Exists(gameWorldMapFullPath)) return false;

            Directory.CreateDirectory(gameWorldMapFullPath);

#warning 完成后作为创建初始数据用

            var gd = new GameData()
            {
                force = new IncreasedIdDictionary<Force>() { map = new Dictionary<int, Force>() },
                province = new IncreasedIdDictionary<Province> { map = new Dictionary<int, Province>() },
                stronghold = new IncreasedIdDictionary<Stronghold> { map = new Dictionary<int, Stronghold>() }
            };

            var tm = new MainTileMap(new TileMap.Size(width, height)) { tiles = new MainMapTile[width * height] };

            var gm = new MainTileMapData()
            {
                territory = new Dictionary<int, int>(),
                road = new Dictionary<int, int>(),
                stronghold = new Dictionary<int, int>(),
                unit = new Dictionary<int, int>(),
            };

            var md = new MasterData()
            {
                terrain = new Dictionary<int, Terrain>()
                {
                    { 0, new Terrain()
                    {
                        id = 0,
                        name = "plain",
                        isWater = false,
                    }
                    },
                    { 1, new Terrain()
                    {
                        id = 1,
                        name = "water",
                        isWater = true,
                    }
                    }
                },
                region = new Dictionary<int, Region>()
                {
                    { 0, new Region()
                    {
                        id = 0,
                        name = "近畿",
                        climate = Climate.normal
                    }
                    }
                },
                culture = new Dictionary<int, Culture>()
                {
                    { 0, new Culture()
                    {
                        id = 0,
                        name = "大和"
                    }
                    }
                },
                religion = new Dictionary<int, Religion>()
                {
                    { 0, new Religion()
                    {
                        id = 0,
                        name = "神道教",
                        isPolytheism = false
                    }
                    }
                },
                road = new Dictionary<int, Road>(),

                strongholdType = new Dictionary<int, Stronghold.Type>()
                {
                    { 1, new Stronghold.Type()
                    {
                        id = 1,
                        name = "城",
                        culture = null,
                        introduction = "介绍"
                    }
                    }
                },

                terrainImage = new Dictionary<int, TerrainImage>()
                {
                    { 0, new TerrainImage()
                    {
                        id = 0,
                        name = "plain",
                        terrainId = 0,
                        animationDetailSpring = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSummer = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailAutumn = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewSpring = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewSummer = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewAutumn = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        }
                    }
                    },
                    { 1, new TerrainImage()
                    {
                        id = 1,
                        name = "water",
                        terrainId = 1,
                        animationDetailSpring = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 144),
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 288),
                            },
                        },
                        animationDetailSummer = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 144),
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 288),
                            },
                        },
                        animationDetailAutumn = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 144),
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 288),
                            },
                        },
                        animationDetailWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 144),
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_water.png",
                                vertex = new Point(0, 288),
                            },
                        },
                        animationViewSpring = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewSummer = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewAutumn = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationViewWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_water.png",
                                vertex = new Point(0, 0)
                            }
                        }
                    }
                    }
                },

                mainTileMapViewImageInfo = new TileMapImageInfo()
                {
                    id = 1,
                    name = "default view map",
                    tileSize = new Size(24, 24),
                },
                mainTileMapDetailImageInfo = new TileMapImageInfo()
                {
                    id = 2,
                    name = "default detail map",
                    tileSize = new Size(48, 48),
                },
                detailTileMapViewImageInfo = new TileMapImageInfo()
                {
                    id = 1,
                    name = "default view map",
                    tileSize = new Size(24, 24),
                },
                detailTileMapDetailImageInfo = new TileMapImageInfo()
                {
                    id = 2,
                    name = "default detail map",
                    tileSize = new Size(48, 48),
                },
                detailTileMapInfo = new Dictionary<int, DetailTileMapInfo>()
                {
                    { 1, new DetailTileMapInfo()
                    {
                        id = 1,
                        name = "plain",
                        size = new TileMap.Size(100, 100),
                        terrainId = 0
                    }
                    }
                }
            };

            saveMapMasterData(new GameWorldMap()
            {
                masterData = md,
                gameData = gd,
                mainTileMap = tm,
                mainTileMapData = new MainTileMapData()
                {
                    territory = new Dictionary<int, int>(),
                    road = new Dictionary<int, int>(),
                    stronghold = new Dictionary<int, int>(),
                    unit = new Dictionary<int, int>()
                }
            });

            return true;
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

        public string getMapFilePath(string fileName, bool isAbsolute = false)
            => isAbsolute ? $"{gameWorldMapFullPath}/{fileName}" : $"{gameWorldMapPath}/{fileName}";

        public void deleteMapMasterData() => Directory.Delete(gameWorldMapFullPath, true);

        public T loadMapMasterData<T>(T gw) where T : GameWorldMap => loadMasterData(gw, gameWorldMapFullPath);

        public T loadGameMasterData<T>(T gw) where T : GameWorldMap => loadMasterData(gw, gameWorldGameFullPath);

        private T loadMasterData<T>(T gw, string path) where T : GameWorldMap
        {
            gw.mainTileMap = File.ReadAllText(path + MainMapName, encoding).uncompressTileMap().fromJson<MainTileMap>();
            gw.mainTileMapData = File.ReadAllText(path + MainMapDataName, encoding).fromJson<MainTileMapData>();
            gw.masterData = File.ReadAllText(path + MasterDataName, encoding).fromJson<MasterData>();

            return gw;
        }

        public void saveMapMasterData(GameWorldMap gw) => saveMasterData(gw, gameWorldMapFullPath);

        private void saveMasterData(GameWorldMap gw, string path)
        {
            File.WriteAllText(path + MainMapName, gw.mainTileMap.toJson().compressTileMap(), encoding);
            File.WriteAllText(path + MainMapDataName, gw.mainTileMapData.toJson(), encoding);
            File.WriteAllText(path + MasterDataName, gw.masterData.toJson(), encoding);
        }

        public DetailTileMap loadDetailTileMap(int id, int width, int height)
        {
            var path = gameWorldMapFullPath + string.Format(DetailMapDataName, id);

            if (!File.Exists(path)) return new DetailTileMap(new TileMap.Size(height, width)) { tiles = new DetailMapTile[width * height] };

            return File.ReadAllText(path, encoding).uncompressTileMap().fromJson<DetailTileMap>();
        }

        public void saveDetailTileMap(int id, DetailTileMap tm)
        {
            var path = gameWorldMapFullPath + string.Format(DetailMapDataName, id);

            File.WriteAllText(path, tm.toJson().compressTileMap(), encoding);
        }
    }
}
