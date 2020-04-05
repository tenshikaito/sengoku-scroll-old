using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
#pragma warning disable CS0660 // 类型定义运算符 == 或运算符 !=，但不重写 Object.Equals(object o)
#pragma warning disable CS0661 // 类型定义运算符 == 或运算符 !=，但不重写 Object.GetHashCode()
    public struct MapPoint
#pragma warning restore CS0661 // 类型定义运算符 == 或运算符 !=，但不重写 Object.GetHashCode()
#pragma warning restore CS0660 // 类型定义运算符 == 或运算符 !=，但不重写 Object.Equals(object o)
    {
        public int x;
        public int y;

        public MapPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"x:{x}, y:{y}";

        public static bool operator ==(MapPoint p1, MapPoint p2) => p1.x == p2.x && p1.y == p2.y;

        public static bool operator !=(MapPoint p1, MapPoint p2) => !(p1 == p2);
    }
}
