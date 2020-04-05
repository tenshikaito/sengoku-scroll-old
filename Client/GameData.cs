using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class GameData
    {
        public Dictionary<int, Terrain> terrain;
        public Dictionary<int, Region> region;
        public Dictionary<int, Culture> culture;
        public Dictionary<int, Religion> religion;
        public Dictionary<int, Road> road;
        public Dictionary<int, Stronghold.Type> strongholdType;

        public IncreasedIdDictionary<Force> force;
        public IncreasedIdDictionary<Province> province;
        public IncreasedIdDictionary<Stronghold> stronghold;
    }

    public class IncreasedIdDictionary<TValue>
    {
        public int id;
        public Dictionary<int, TValue> map;

        public TValue this[int index] => map[index];
    }
}
