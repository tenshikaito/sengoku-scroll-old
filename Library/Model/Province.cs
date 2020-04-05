using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class Province : BaseObject
    {
        public int id;

        public string name;

        public int? forceId;

        public string introduction;

        public List<int> strongholds;
    }
}
