using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class IncreasedIdDictionary<TValue>
    {
        public int id;
        public Dictionary<int, TValue> map;

        public TValue this[int index]
        {
            get => map[index];
            set => map[index] = value;
        }

        public IncreasedIdDictionary(int startId = 1)
        {
            id = startId;
        }

        public IncreasedIdDictionary<TValue> init()
        {
            map = new Dictionary<int, TValue>();

            return this;
        }

        public int getNextId() => id++; 
    }
}
