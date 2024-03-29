﻿using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Model
{
    public class Region : BaseObject
    {
        public static readonly List<Climate> list = CommonHelper.getEnumList<Climate>();

        public int id;
        public string name;
        /// <summary>气候</summary>
        public Climate climate;
    }
}
