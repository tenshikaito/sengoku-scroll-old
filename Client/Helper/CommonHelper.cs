using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Client.Helper
{
    public static class CommonHelper
    {
        public static void dragCamera(this Camera c, Point p)
        {
            c.x -= p.X;
            c.y -= p.Y;
        }

        public static string getSymbol(this bool value, Wording w) => value ? w.symbol_selected : w.symbol_unselected;

        public static Image getImage(this GameWorld gw, string fileName) => gw.cache.getImage(gw.getGameWorldProcessor().getFilePath(fileName));
    }
}
