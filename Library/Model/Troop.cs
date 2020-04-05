using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class Troop  : Unit
    {
        public Type type;

        public enum Type : byte
        {
            Army = 0,
            Labour = 1,
            Transporter = 2,
            Sattler = 3
        }
    }
}
