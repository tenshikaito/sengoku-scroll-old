using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphic
{
    public class ZoomableTileMapSprites<T> : GameObject where T : TileMapSpritesBase
    {
        private List<T> tileMapSpritesList;
        private int tileMapIndex = 0;

        public T outerTileMap => tileMapSpritesList[tileMapIndex];

        public ZoomableTileMapSprites(List<T> list)
        {
            tileMapSpritesList = list;

            children.AddRange(list);

            previous();
        }

        public bool next()
        {
            hideAll();

            var count = tileMapSpritesList.Count;

            if (count == 0) return false;

            var flag = true;

            if (++tileMapIndex >= count)
            {
                tileMapIndex = count - 1;
                flag = false;
            }

            showCurrent();

            return flag;
        }

        public bool previous()
        {
            hideAll();

            if (tileMapSpritesList.Count == 0) return false;

            var flag = true;

            if (--tileMapIndex < 0)
            {
                tileMapIndex = 0;
                flag = false;
            }

            showCurrent();

            return flag;
        }

        private void showCurrent() => tileMapSpritesList[tileMapIndex].isEnable = true;

        private void hideAll() => tileMapSpritesList.ForEach(o => o.isEnable = false);
    }
}
