﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library.Model
{
    public abstract class Unit : TileObject
    {
        public string name;
        public int? force;
        public int? stronghold;
        public int commander;
        public int hp;
        public Direction4 direction = Direction4.down;
        public bool isShip;
        public bool isChaos;
    }
}
