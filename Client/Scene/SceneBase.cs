using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client.Scene
{
    public abstract class SceneBase : GameObject //GameStatusAdapter
    {
        protected GameSystem gameSystem;

        protected FormMain formMain => gameSystem.formMain;

        protected FormMain.Dispatcher dispatcher => formMain.dispatcher;

        public SceneBase(GameSystem gs) => gameSystem = gs;

        protected void Invoke(Action a) => formMain.dispatcher.invoke(a);
    }
}
