using Library;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Command
{
    public class LoadGameCommand : CommandBase
    {
        public LoadGameCommand(Game g) : base(g)
        {
        }

        public override async Task execute(GameClient gc, string data)
        {
            var gw = game.gameWorld;

            var ngw = new GameWorldMap(game.gameWorldName)
            {
                gameDate = gw.gameDate,
                masterData = gw.masterData,
                gameData = gw.gameData,
                mainTileMap = gw.mainTileMap,
            };
        }
    }
}
