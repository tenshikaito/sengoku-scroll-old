using Library;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Command
{
    public class TestServerCommand : CommandBase
    {
        public TestServerCommand(Game g) : base(g)
        {
        }

        public override async Task execute(GameClient gc, string data)
        {
            var d = data.fromJson<TestServerData>();

            try
            {
                await gc.write(nameof(TestServerCommand), d);

                gc.disconnect();

                game.disconnect(gc);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
