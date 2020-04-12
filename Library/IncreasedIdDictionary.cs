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

        public TValue this[int index] => map[index];
    }
}
