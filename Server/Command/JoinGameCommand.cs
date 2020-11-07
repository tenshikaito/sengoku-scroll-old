using Library;
using Library.Helper;
using Library.Model;
using Library.Network;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Command
{
    public class JoinGameCommand : CommandBase
    {
        public JoinGameCommand(GameService g) : base(g)
        {
        }

        public override async Task execute(GameClient gc, string data)
        {
            var request = data.fromJson<JoinGameRequestData>();

            var gw = game.gameWorld;
            var gwd = gw.gameWorldData;

            var ngw = new GameWorldData(game.gameWorldName)
            {
                resourcePackageName = gwd.resourcePackageName,
                gameDate = gwd.gameDate,
                masterData = gwd.masterData,
                gameData = gwd.gameData,
                tileMap = gwd.tileMap,
            };

            var p = game.gameWorld.gameWorldData.gameData.player.map.Values.SingleOrDefault(o => o.code == request.playerCode);

            if (p == null)
            {
                var gp = game.gameWorld.player.map.Values.SingleOrDefault(o => o.code == request.playerCode) ?? new GamePlayer()
                {
                    id = game.gameWorld.player.getNextId(),
                    playerName = request.playerName,
                    code = request.playerCode,
                    gameClient = gc,
                    player = p = new Player()
                    {
                        id = game.gameWorld.gameWorldData.gameData.player.getNextId(),
                        code = request.playerCode
                    }.init()
                };

                game.gameWorld.gameWorldData.gameData.player[p.id] = p;
                game.gameWorld.player[gp.id] = gp;
            }

            var response = new JoinGameResponseData()
            {
                gameWorldData = ngw,
            };

            await gc.write(response.toJson());
        }
    }
}
