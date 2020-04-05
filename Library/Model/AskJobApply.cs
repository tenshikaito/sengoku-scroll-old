using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class AskJobApply
    {
        public int id { get; set; }
        public bool isInvalid { get; set; }
        public int characterId;
        public DateTime time;
        public string comment;
    }
}
