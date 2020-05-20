using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public static class GameHelper
    {
        public static string compressTileMap(this string json)
            => json.Replace($@"""{ nameof(MainMapTile.terrain) }""", @"""t""")
            .Replace($@"""{ nameof(MainMapTile.region) }""", @"""r""")
            .Replace($@"""{ nameof(DetailMapTile.height) }""", @"""h""")
            .Replace($@"""{ nameof(DetailMapTile.functionType) }""", @"""f""");

        public static string uncompressTileMap(this string json)
            => json.Replace(@"""t""", $@"""{ nameof(MainMapTile.terrain) }""")
            .Replace(@"""r""", $@"""{ nameof(MainMapTile.region) }""")
            .Replace(@"""h""", $@"""{ nameof(DetailMapTile.height) }""")
            .Replace(@"""f""", $@"""{ nameof(DetailMapTile.functionType) }""");

        public static List<TileAnimationFrame> toAnimationFrameList(this string text)
            => string.IsNullOrWhiteSpace(text)
            ? null
            : text.Replace(Environment.NewLine, string.Empty).Trim(';').Split(';').Select(o =>
            {
                var arr = o.Split(':');
                var vertex = arr[1].Split(',');

                return new TileAnimationFrame()
                {
                    fileName = arr[0],
                    vertex = new Point(int.Parse(vertex[0]), int.Parse(vertex[1]))
                };
            }).ToList();

        public static string toString(this List<TileAnimationFrame> list)
            => string.Join(
                ";" + Environment.NewLine,
                list == null
                ? new string[0]
                : list.Select(o => $"{o.fileName}:{o.vertex.X},{o.vertex.Y}").ToArray());
    }
}
