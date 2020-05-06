using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class MasterData
    {
        public Dictionary<int, Terrain> terrain;
        public Dictionary<int, Region> region;
        public Dictionary<int, Culture> culture;
        public Dictionary<int, Religion> religion;
        public Dictionary<int, Road> road;
        public Dictionary<int, Stronghold.Type> strongholdType;

        public Dictionary<int, TerrainImage> terrainImage;
        public Dictionary<int, DetailTileMapInfo> detailTileMapInfo;

        public MainTileMapImageInfo mainTileMapViewImageInfo;
        public MainTileMapImageInfo mainTileMapDetailImageInfo;
        public DetailTileMapImageInfo detailTileMapViewImageInfo;
        public DetailTileMapImageInfo detailTileMapDetailImageInfo;
    }
}
