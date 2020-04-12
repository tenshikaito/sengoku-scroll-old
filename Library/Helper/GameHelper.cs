using Library.Model;
using System;
using System.Collections.Generic;
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
    }
}
