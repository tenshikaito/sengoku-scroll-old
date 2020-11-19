using Library;
using Library.Helper;
using Library.Model;
using Library.Network;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // 尝试取出对应玩家
            var p = game.gameWorld.gameWorldData.gameData.player.map.Values.SingleOrDefault(o => o.code == request.playerCode);

            // 如果不存在就创建玩家
            if (p == null)
            {
                p = new Player()
                {
                    id = game.gameWorld.gameWorldData.gameData.player.getNextId(),
                    code = request.playerCode,
                    name = request.playerName
                }.init();
            }

            // 如果不存在就创建玩家连接
            var gp = game.gameWorld.player.map.Values.SingleOrDefault(o => o.code == request.playerCode);

            if (gp != null)
            {
                try
                {
                    gp.gameClient.disconnect();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            gp = gp ?? new GamePlayer()
            {
                id = game.gameWorld.player.getNextId(),
                playerName = request.playerName,
                code = request.playerCode,
                player = p
            };

            gp.gameClient = gc;

            game.gameWorld.gameWorldData.gameData.player[p.id] = p;
            game.gameWorld.player[gp.id] = gp;

            var response = new JoinGameResponseData()
            {
                gameWorldData = ngw,
                player = p
            };

            await gc.write(response.toJson());
        }
    }
}
