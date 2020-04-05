using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public enum Relationship : byte
    {
        none = 0,
        self = 1,
        normal = 2,
        ally = 3,
        enemy = 4
    }
}
