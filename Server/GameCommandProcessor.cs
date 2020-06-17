using Library;
using Library.Helper;
using Server.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameCommandProcessor
    {
        private Dictionary<string, CommandBase> map = new Dictionary<string, CommandBase>();

        public GameCommandProcessor(Game g)
        {
            map[nameof(TestServerCommand)] = new TestServerCommand(g);
        }

        public void execute(GameClient gc, string name, string data) => map[name].execute(gc, data);
    }
}
