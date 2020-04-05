using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class Religion : BaseObject
    {
        public byte id;
        public string name;
        public string code;
        public bool isPolytheism;

        public class Sect
        {
            public int id;
            public string name;
            public int religionId;
            public int creatorId;
        }
    }
}
