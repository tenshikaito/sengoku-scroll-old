using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphic
{
    public static class AutoTileCalculator
    {
        public const byte left = 1;
        public const byte right = left * 2;
        public const byte top = right * 2;
        public const byte bottom = top * 2;
        public const byte topLeft = bottom * 2;
        public const byte topRight = topLeft * 2;
        public const byte bottomLeft = topRight * 2;
        public const byte bottomRight = bottomLeft * 2;

        public static ICalculator cTopLeft = new TopLeftCalculator();
        public static ICalculator cTopRight = new TopRightCalculator();
        public static ICalculator cBottomLeft = new BottomLeftCalculator();
        public static ICalculator cBottomRight = new BottomRightCalculator();

        public abstract class ICalculator
        {
            public abstract (int x, int y) calculate(
                int vertexX, int vertexY, int tileWidth, int tileHeight, int width, int height, byte different);
        }

        protected class TopLeftCalculator : ICalculator
        {
            public override (int x, int y) calculate(
                int vertexX, int vertexY, int tileWidth, int tileHeight, int width, int height, byte different)
            {
                var x = vertexX + width;
                var y = vertexY + height + tileHeight;
                if ((different & (left | top)) == (left | top))
                {
                    x -= width;
                    y -= height;
                }
                else if ((different & left) == left)
                {
                    x -= width;
                }
                else if ((different & top) == top)
                {
                    y -= height;
                }
                else if ((different & topLeft) == topLeft)
                {
                    x = vertexX + tileWidth;
                    y = vertexY;
                }
                else
                {
                    x = vertexX + tileWidth;
                    y = vertexY + tileHeight * 2;
                }

                return (x, y);
            }
        }

        protected class TopRightCalculator : ICalculator
        {
            public override (int x, int y) calculate(
                int vertexX, int vertexY, int tileWidth, int tileHeight, int width, int height, byte different)
            {
                var x = vertexX + tileWidth;
                var y = vertexY + height + tileHeight;
                if ((different & (top | right)) == (top | right))
                {
                    x += width;
                    y -= height;
                }
                else if ((different & right) == right)
                {
                    x += width;
                }
                else if ((different & top) == top)
                {
                    y -= height;
                }
                else if ((different & topRight) == topRight)
                {
                    x = vertexX + tileWidth + width;
                    y = vertexY;
                }
                else
                {
                    x = vertexX + width;
                    y = vertexY + tileHeight * 2;
                }

                return (x, y);
            }
        }

        protected class BottomLeftCalculator : ICalculator
        {
            public override (int x, int y) calculate(
                int vertexX, int vertexY, int tileWidth, int tileHeight, int width, int height, byte different)
            {
                var x = vertexX + width;
                var y = vertexY + tileHeight * 2;
                if ((different & (bottom | left)) == (bottom | left))
                {
                    x -= width;
                    y += height;
                }
                else if ((different & left) == left)
                {
                    x -= width;
                }
                else if ((different & bottom) == bottom)
                {
                    y += height;
                }
                else if ((different & bottomLeft) == bottomLeft)
                {
                    x = vertexX + tileWidth;
                    y = vertexY + height;
                }
                else
                {
                    x = vertexX + tileWidth;
                    y = vertexY + height + tileHeight;
                }

                return (x, y);
            }
        }

        protected class BottomRightCalculator : ICalculator
        {
            public override (int x, int y) calculate(
                int vertexX, int vertexY, int tileWidth, int tileHeight, int width, int height, byte different)
            {
                var x = vertexX + tileWidth;
                var y = vertexY + tileHeight * 2;
                if ((different & (bottom | right)) == (bottom | right))
                {
                    x += width;
                    y += height;
                }
                else if ((different & right) == right)
                {
                    x += width;
                }
                else if ((different & bottom) == bottom)
                {
                    y += height;
                }
                else if ((different & bottomRight) == bottomRight)
                {
                    x = vertexX + tileWidth + width;
                    y = vertexY + height;
                }
                else
                {
                    x = vertexX + width;
                    y = vertexY + height + tileHeight;
                }

                return (x, y);
            }
        }
    }
}
