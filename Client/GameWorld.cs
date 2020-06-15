using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class GameWorld : GameWorldMap
    {
        public GameOption gameOption = new GameOption();

        public Player currentPlayer;

        public Camera camera;

        public Cache cache = new Cache();

        public GameWorld(string name) : base(name)
        {
        }
    }
}
