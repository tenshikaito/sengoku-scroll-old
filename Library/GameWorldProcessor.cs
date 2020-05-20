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

            var tm = new MainTileMap(new TileMap.Size(width, height))
            {
                tiles = new MainMapTile[width * height],
                terrain = new Dictionary<int, byte>()
            };

            var gm = new MainTileMapData()
            {
                territory = new Dictionary<int, int>(),
                road = new Dictionary<int, int>(),
                stronghold = new Dictionary<int, int>(),
                unit = new Dictionary<int, int>(),
            };

            var md = new MasterData()
            {
                mainTileMapTerrain = new Dictionary<int, Terrain>()
                {
                    { 0, new Terrain()
                    {
                        id = 0,
                        name = "shallow_sea",
                        imageId = 0,
                        isWater = true,
                    }
                    },
                    { 1, new Terrain()
                    {
                        id = 1,
                        name = "deep_sea",
                        imageId = 1,
                        isSurface = true,
                        isWater = true,
                        isDeepWater = true,
                    }
                    },
                    { 2, new Terrain()
                    {
                        id = 2,
                        name = "lake",
                        imageId = 0,
                        isWater = true,
                        isFreshWater = true
                    }
                    },
                    { 3, new Terrain()
                    {
                        id = 3,
                        name = "deep_lake",
                        imageId = 1,
                        isSurface = true,
                        isWater = true,
                        isFreshWater = true,
                        isDeepWater = true
                    }
                    },
                    { 4, new Terrain()
                    {
                        id = 4,
                        name = "river",
                        imageId = 0,
                        isWater = true,
                        isFreshWater = true,
                    }
                    },
                    { 5, new Terrain()
                    {
                        id = 5,
                        name = "deep river",
                        imageId = 1,
                        isSurface = true,
                        isWater = true,
                        isFreshWater = true,
                        isDeepWater = true
                    }
                    },
                    { 6, new Terrain()
                    {
                        id = 6,
                        name = "plain",
                        imageId = 2,
                    }
                    },
                    { 7, new Terrain()
                    {
                        id = 7,
                        name = "grassland",
                        imageId = 3,
                        isSurface = true,
                        isGrass = true
                    }
                    },
                    { 8, new Terrain()
                    {
                        id = 8,
                        name = "forest",
                        imageId = 4,
                        isSurface = true,
                        isForest = true
                    }
                    },
                    { 9, new Terrain()
                    {
                        id = 9,
                        name = "forest",
                        imageId = 5,
                        isSurface = true,
                        isForest = true
                    }
                    },
                    { 10, new Terrain()
                    {
                        id = 10,
                        name = "forest",
                        imageId = 6,
                        isSurface = true,
                        isForest = true
                    }
                    },
                    { 11, new Terrain()
                    {
                        id = 11,
                        name = "coconut grove",
                        imageId = 7,
                        isSurface = true,
                        isForest = true
                    }
                    },
                    { 12, new Terrain()
                    {
                        id = 12,
                        name = "hill",
                        isHill = true,
                        imageId = 8,
                        isSurface = true,
                    }
                    },
                    { 13, new Terrain()
                    {
                        id = 13,
                        name = "mountain",
                        imageId = 9,
                        isSurface = true,
                        isMountain = true
                    }
                    },
                    { 14, new Terrain()
                    {
                        id = 14,
                        name = "mountain",
                        imageId = 10,
                        isSurface = true,
                        isMountain = true
                    }
                    },
                    { 15, new Terrain()
                    {
                        id = 15,
                        name = "wasteland",
                        imageId = 11,
                        isWaste = true
                    }
                    },
                    { 16, new Terrain()
                    {
                        id = 16,
                        name = "wasteland",
                        imageId = 12,
                        isSurface = true,
                        isWaste = true
                    }
                    },
                    { 17, new Terrain()
                    {
                        id = 17,
                        name = "desert",
                        imageId = 13,
                        isDesert = true
                    }
                    },
                    { 18, new Terrain()
                    {
                        id = 18,
                        name = "desert",
                        imageId = 14,
                        isSurface = true,
                        isDesert = true
                    }
                    },
                    { 19, new Terrain()
                    {
                        id = 19,
                        name = "snowland",
                        imageId = 15,
                        isMarsh = true
                    }
                    },
                    { 20, new Terrain()
                    {
                        id = 20,
                        name = "marsh",
                        imageId = 16,
                        isMarsh = true
                    }
                    },
                    { 21, new Terrain()
                    {
                        id = 21,
                        name = "rock",
                        imageId = 17,
                        isSurface = true,
                        isRock = true
                    }
                    },
                    { 22, new Terrain()
                    {
                        id = 22,
                        name = "rock",
                        imageId = 18,
                        isSurface = true,
                        isRock = true,
                        isWater = true
                    }
                    },
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
                        name = "shallow_water",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_shallow_water.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_shallow_water.png",
                                vertex = new Point(96, 0),
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_shallow_water.png",
                                vertex = new Point(192, 0),
                            },
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_shallow_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 1, new TerrainImage()
                    {
                        id = 1,
                        name = "deep_water",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_deep_water_1.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_deep_water_2.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_deep_water_3.png",
                                vertex = new Point(0, 0)
                            },
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_deep_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 2, new TerrainImage()
                    {
                        id = 2,
                        name = "plain",
                        animationDetail = new List<TileAnimationFrame>()
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
                                fileName = "detail_terrain_plain_summer.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_snow.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_plain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 3, new TerrainImage()
                    {
                        id = 3,
                        name = "grass",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_grass.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSummer = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_grass_summer.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_none.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_grass.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 4, new TerrainImage()
                    {
                        id = 4,
                        name = "broad-leaved forest",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_3.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_forest.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 5, new TerrainImage()
                    {
                        id = 5,
                        name = "coniferous forest",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_4.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_forest.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 6, new TerrainImage()
                    {
                        id = 6,
                        name = "withered forest",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_3.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_forest.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 7, new TerrainImage()
                    {
                        id = 7,
                        name = "coconut grove",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_forest_5.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_forest.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 8, new TerrainImage()
                    {
                        id = 8,
                        name = "hill",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_hill_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailAutumn = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_hill_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailWinter = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_hill_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_hill_3.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_hill.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 9, new TerrainImage()
                    {
                        id = 9,
                        name = "mountain 1",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_mountain_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_mountain_5.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_mountain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 10, new TerrainImage()
                    {
                        id = 10,
                        name = "mountain 2",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_mountain_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_mountain_5.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_mountain.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 11, new TerrainImage()
                    {
                        id = 11,
                        name = "wasteland 1",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_wasteland_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_snow.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_wasteland_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 12, new TerrainImage()
                    {
                        id = 12,
                        name = "wasteland 2",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_wasteland_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationDetailSnow = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_none.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_wasteland_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 13, new TerrainImage()
                    {
                        id = 13,
                        name = "desert 1",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_desert_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_desert_1.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 14, new TerrainImage()
                    {
                        id = 14,
                        name = "desert 2",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_desert_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_desert_2.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 15, new TerrainImage()
                    {
                        id = 15,
                        name = "snow",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_snow.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_snow.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 16, new TerrainImage()
                    {
                        id = 16,
                        name = "marsh",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_marsh_1.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_marsh_2.png",
                                vertex = new Point(0, 0)
                            },
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_marsh_3.png",
                                vertex = new Point(0, 0)
                            },
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_marsh.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 17, new TerrainImage()
                    {
                        id = 17,
                        name = "rock",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_rock.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_rock.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
                    { 18, new TerrainImage()
                    {
                        id = 18,
                        name = "rock water",
                        animationDetail = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "detail_terrain_rock_water.png",
                                vertex = new Point(0, 0)
                            }
                        },
                        animationView = new List<TileAnimationFrame>()
                        {
                            new TileAnimationFrame()
                            {
                                fileName = "view_terrain_rock.png",
                                vertex = new Point(0, 0)
                            }
                        },
                    }
                    },
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
