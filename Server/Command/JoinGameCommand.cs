using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Command
{
    public class JoinGameCommand : CommandBase
    {
        public JoinGameCommand(Game g) : base(g)
        {
        }

        public override async Task execute(GameClient gc, string data)
        {
            var request = data.fromJson<JoinGameRequestData>();

            var gw = game.gameWorld;

            var ngw = new GameWorldMap(game.gameWorldName)
            {
                resourcePackageName = gw.resourcePackageName,
                gameDate = gw.gameDate,
                masterData = gw.masterData,
                gameData = gw.gameData,
                mainTileMap = gw.mainTileMap,
            };

            var response = new JoinGameResponseData()
            {
                gameWorldMap = ngw,
            };

            await gc.write(response.toJson());
        }
    }
}
