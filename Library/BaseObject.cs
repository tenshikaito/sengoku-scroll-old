using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class BaseObject
    {
        public static implicit operator bool(BaseObject o) => o != null;
    }
}
