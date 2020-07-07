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
    public class CommandProcessor
    {
        private Dictionary<string, CommandBase> commandMap  ;

        public CommandProcessor(Dictionary<string, CommandBase> commandMap)
        {
            this.commandMap = commandMap;
        }

        public async ValueTask<bool> execute(GameClient gc, string name, string data)
        {
            if (commandMap.TryGetValue(name, out var c))
            {
                await c.execute(gc, data);

                return true;
            }

            return false;
        }
    }
}
