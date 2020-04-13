using Library;
using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Region = Library.Model.Region;

namespace Client
{
    public class GameWorldProcessor
    {
        private const string DirName = "Map";

        private const string MainMapName = "/main_map_.dat";
        private const string MainMapDataName = "/main_map_data.dat";
        private const string DetailMapName = "/detail_map_{0}.dat";
        private const string DetailMapDataName = "/detail_map_data_{0}.dat";
        private const string MasterDataName = "/data.dat";

        public static readonly Encoding encoding = Encoding.UTF8;

        private string gameWorldName;
        private string gameWorldPath;
        private string gameWorldFullPath;

        public GameWorldProcessor(string gameWorldName)
        {
            this.gameWorldName = gameWorldName;

            gameWorldPath = $"{DirName}/{gameWorldName}";
            gameWorldFullPath = $"{Directory.GetCurrentDirectory()}/{gameWorldPath}";
        }

        public static List<string> getGameWorldList()
        {
            var currentDirPath = Directory.GetCurrentDirectory();
            var path = $"{currentDirPath}/{DirName}";

            Directory.CreateDirectory(path);

            return Directory.EnumerateDirectories(path).Select(o => new DirectoryInfo(o).Name).ToList();
        }

        public bool createGameWorldDirectory(int width, int height)
        {
            if (Directory.Exists(gameWorldFullPath)) return false;

            Directory.CreateDirectory(gameWorldFullPath);

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
                        isWater = false
                    }
                    },
                    { 1, new Terrain()
                    {
                        id = 1,
                        name = "water",
                        isWater = true
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
                mainTileMapImageInfo = new Dictionary<int, MainTileMapImageInfo>()
                {
                    { 1, new MainTileMapImageInfo()
                    {
                        id = 1,
                        name="default view map",
                        tileSize = new Size(24, 24),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            }
                        },
                        strongholdAnimation = new Dictionary<int, TileAnimation>()
                        {
                        }
                    }
                    },
                    //{ 2, new MainTileMapImageInfo()
                    //{
                    //    id = 2,
                    //    name="default detail map",
                    //    tileSize = new Size(48, 48),
                    //    terrainAnimation = new Dictionary<byte, TileAnimation>()
                    //    {
                    //        { 0, new TileAnimation()
                    //        {
                    //            id = 0,
                    //            interval = 0.5f,
                    //            frames= new List<TileAnimation.Frame>()
                    //            {
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName = "detail_terrain_plain.png",
                    //                    vertex = new Point(0, 0)
                    //                }
                    //            }
                    //        }
                    //        },
                    //        { 1, new TileAnimation()
                    //        {
                    //            id = 1,
                    //            interval = 0.5f,
                    //            frames= new List<TileAnimation.Frame>()
                    //            {
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName = "detail_terrain_water.png",
                    //                    vertex = new Point(0, 0)
                    //                },
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName="detail_terrain_water.png",
                    //                    vertex = new Point(0, 144),
                    //                },
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName="detail_terrain_water.png",
                    //                    vertex = new Point(0, 288),
                    //                },
                    //            }
                    //        }
                    //        }
                    //    },
                    //    strongholdAnimation = new Dictionary<int, TileAnimation>()
                    //    {
                    //    }
                    //}
                    //}
                },
                detailTileMapImageInfo = new Dictionary<int, DetailTileMapImageInfo>() {
                    { 1, new DetailTileMapImageInfo()
                    {
                        id = 1,
                        name="default view map",
                        tileSize = new Size(24, 24),
                        terrainAnimation = new Dictionary<byte, TileAnimation>()
                        {
                            { 0, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_plain.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            },
                            { 1, new TileAnimation()
                            {
                                id = 0,
                                interval = 0.5f,
                                frames = new List<TileAnimation.Frame>()
                                {
                                    new TileAnimation.Frame()
                                    {
                                        fileName = "view_terrain_water.png",
                                        vertex = new Point(0, 0)
                                    }
                                }
                            }
                            }
                        }
                    }
                    },
                    //{ 2, new DetailTileMapImageInfo()
                    //{
                    //    id = 2,
                    //    name="default detail map",
                    //    tileSize = new Size(48, 48),
                    //    terrainAnimation = new Dictionary<byte, TileAnimation>()
                    //    {
                    //        { 0, new TileAnimation()
                    //        {
                    //            id = 0,
                    //            interval = 0.5f,
                    //            frames= new List<TileAnimation.Frame>()
                    //            {
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName = "detail_terrain_plain.png",
                    //                    vertex = new Point(0, 0)
                    //                }
                    //            }
                    //        }
                    //        },
                    //        { 1, new TileAnimation()
                    //        {
                    //            id = 1,
                    //            interval = 0.5f,
                    //            frames= new List<TileAnimation.Frame>()
                    //            {
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName = "detail_terrain_water.png",
                    //                    vertex = new Point(0, 0)
                    //                },
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName="detail_terrain_water.png",
                    //                    vertex = new Point(0, 144),
                    //                },
                    //                new TileAnimation.Frame()
                    //                {
                    //                    fileName="detail_terrain_water.png",
                    //                    vertex = new Point(0, 288),
                    //                },
                    //            }
                    //        }
                    //        }
                    //    },
                    //}
                    //}
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

            save(new GameWorld(gameWorldName)
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

            // todo 数据固定后将所有文件全部.dat数据文件添加到目标文件夹

            Directory.GetFiles($"{Directory.GetCurrentDirectory()}/Data/GameWorld").ToList().ForEach(o =>
            {
                var fi = new FileInfo(o);
                File.Copy(o, $"{gameWorldFullPath}/{fi.Name}");
            });

            return true;
        }

        public string getFilePath(string fileName, bool isAbsolute = false)
            => isAbsolute ? $"{gameWorldPath}/{fileName}" : $"{gameWorldFullPath}/{fileName}";

        public void delete() => Directory.Delete(gameWorldFullPath, true);

        public GameWorld load(int screenWidth, int screenHeight)
        {
            var gw = new GameWorld(gameWorldName)
            {
                camera = new Camera(screenWidth, screenHeight)
            };

            gw.mainTileMap = File.ReadAllText(gameWorldFullPath + MainMapName, encoding).uncompressTileMap().fromJson<MainTileMap>();
            gw.mainTileMapData = File.ReadAllText(gameWorldFullPath + MainMapDataName, encoding).fromJson<MainTileMapData>();
            gw.masterData = File.ReadAllText(gameWorldFullPath + MasterDataName, encoding).fromJson<MasterData>();

            return gw;
        }

        public void save(GameWorld gw)
        {
            File.WriteAllText(gameWorldFullPath + MainMapName, gw.mainTileMap.toJson().compressTileMap(), encoding);
            File.WriteAllText(gameWorldFullPath + MainMapDataName, gw.mainTileMapData.toJson(), encoding);
            File.WriteAllText(gameWorldFullPath + MasterDataName, gw.masterData.toJson(), encoding);
        }

        public DetailTileMap loadDetailTileMap(int id, int width, int height)
        {
            var path = gameWorldFullPath + string.Format(DetailMapDataName, id);

            if (!File.Exists(path)) return new DetailTileMap(new TileMap.Size(height, width)) { tiles = new DetailMapTile[width * height] };

            return File.ReadAllText(path, encoding).uncompressTileMap().fromJson<DetailTileMap>();
        }

        public void saveDetailTileMap(int id, DetailTileMap tm)
        {
            var path = gameWorldFullPath + string.Format(DetailMapDataName, id);

            File.WriteAllText(path, tm.toJson().compressTileMap(), encoding);
        }
    }
}
