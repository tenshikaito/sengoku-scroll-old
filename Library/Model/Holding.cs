using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class Holding : BaseObject
    {
        public int id;
        public string name;
        public int weight { get; set; }
        public int volume { get; set; }
    }
}
