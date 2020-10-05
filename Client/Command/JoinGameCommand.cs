using Client.Game;
using Client.Model;
using Client.Scene;
using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Client.FormMain;

namespace Client.Command
{
    public class JoinGameCommand
    {
        public static async Task execute(ServerInfo si, PlayerInfo pi, SceneWaiting sw, GameSystem gs)
        {
            var w = gs.wording;

            var dispatcher = gs.formMain.dispatcher;

            dispatcher.invoke(() => sw.setText($"{w.scene_title.connecting}..."));

            await NetworkHelper.connect(si.ip, si.port, async (tc, ns) =>
            {
                dispatcher.invoke(() => sw.setText($"{w.scene_title.loading}..."));

                var gc = NetworkHelper.getGameClient(tc);

                await gc.write(new JoinGameRequestData()
                {
                    playerCode = pi.code,
                    playerName = pi.name
                }.toCommandString(nameof(JoinGameCommand)));

                var (hasResult, data) = await gc.read();

                var c = data.fromJson<JoinGameResponseData>();

                var gwd = c.gameWorldData;

                var gw = new GameWorld(gwd.name);

                gw.camera = new Camera(gs.screenWidth, gs.screenHeight);

                gw.init(gwd);

                gs.dispatchSceneToGame(gw);
            });
        }
    }
}
