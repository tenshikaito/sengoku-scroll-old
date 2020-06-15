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
        private GamePlayer gamePlayer;

        public LoadGameCommand(Game g, GamePlayer gp) : base(g)
        {
            gamePlayer = gp;
        }

        public override void execute()
        {
            @lock.EnterReadLock();

            var gw = game.gameWorld;

            var ngw = new GameWorldMap(game.gameWorldName)
            {
                gameDate = gw.gameDate,
                masterData = gw.masterData,
                gameData = gw.gameData,
                mainTileMap = gw.mainTileMap,
            };

            var data = ngw.toJson();

            _ = gamePlayer.gameClient.write(data);

            @lock.ExitReadLock();
        }
    }
}
