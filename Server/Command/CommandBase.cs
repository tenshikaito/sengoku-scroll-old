using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Command
{
    public abstract class CommandBase
    {
        protected Game game;

        public CommandBase(Game g) => game = g;

        public abstract Task execute(GameClient gc, string data);
    }
}
