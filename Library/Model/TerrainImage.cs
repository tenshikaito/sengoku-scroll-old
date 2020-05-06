using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class TerrainImage
    {
        public int id;

        public string name;

        public int terrainId;

        public List<TileAnimationFrame> animationDetailSpring;
        public List<TileAnimationFrame> animationDetailSummer;
        public List<TileAnimationFrame> animationDetailAutumn;
        public List<TileAnimationFrame> animationDetailWinter;

        public List<TileAnimationFrame> animationViewSpring;
        public List<TileAnimationFrame> animationViewSummer;
        public List<TileAnimationFrame> animationViewAutumn;
        public List<TileAnimationFrame> animationViewWinter;
    }
}
