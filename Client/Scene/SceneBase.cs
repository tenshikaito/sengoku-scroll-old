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

        protected FormMain dispatcher => formMain;

        public SceneBase(GameSystem gs) => gameSystem = gs;

        protected void Invoke(Action a) => formMain.invoke(a);
    }
}
