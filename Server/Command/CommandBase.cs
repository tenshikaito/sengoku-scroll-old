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

        protected ReaderWriterLockSlim @lock => game.@lock;

        public CommandBase(Game g)
        {
            game = g;
        }

        public abstract void execute();
    }
}
