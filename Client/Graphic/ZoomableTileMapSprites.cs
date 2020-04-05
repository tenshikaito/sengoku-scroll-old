using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphic
{
    public class ZoomableTileMapSprites<T> : GameObject
        where T : TileMapSpritesBase
    {
        private List<T> outerTileMapSpritesList;
        private int tileMapIndex = 0;

        public T outerTileMap => outerTileMapSpritesList[tileMapIndex];

        public ZoomableTileMapSprites(List<T> list)
        {
            outerTileMapSpritesList = list;

            children.AddRange(list);
            
            previous();
        }

        public bool next()
        {
            hideAll();

            var count = outerTileMapSpritesList.Count;

            if (count == 0) return false;

            var flag = true;

            if (++tileMapIndex >= count)
            {
                tileMapIndex = count - 1;
                flag = false;
            }

            outerTileMapSpritesList[tileMapIndex].isEnable = true;

            return flag;
        }

        public bool previous()
        {
            hideAll();

            if (outerTileMapSpritesList.Count == 0) return false;

            var flag = true;

            if (--tileMapIndex < 0)
            {
                tileMapIndex = 0;
                flag = false;
            }

            outerTileMapSpritesList[tileMapIndex].isEnable = true;

            return flag;
        }

        private void hideAll()
        {
            outerTileMapSpritesList.ForEach(o => o.isEnable = false);
        }
    }
}
