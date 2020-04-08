﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public interface IGameStatus : IInputEventListener
    {
        void start();

        void update();

        void draw();

        void finish();
    }

    public class GameStatusAdapter : IGameStatus
    {
        public virtual void start()
        {
        }

        public virtual void update()
        {
        }

        public virtual void draw()
        {
        }

        public virtual void finish()
        {
        }

        public virtual void mouseMoved(MouseEventArgs e)
        {
        }

        public virtual void mouseDragging(MouseEventArgs e, Point p)
        {
        }

        public virtual void mouseEntered(MouseEventArgs e)
        {
        }

        public virtual void mouseExited(MouseEventArgs e)
        {
        }

        public virtual void mouseClicked(MouseEventArgs e)
        {
        }

        public virtual void mousePressed(MouseEventArgs e)
        {
        }

        public virtual void mouseReleased(MouseEventArgs e)
        {
        }

        public virtual void mouseWheelScrolled(MouseEventArgs e)
        {
        }

        public virtual void keyPressed(KeyPressEventArgs e)
        {
        }

        public virtual void onKeyPressing(KeyEventArgs e)
        {
        }

        public virtual void keyReleased(KeyEventArgs e)
        {
        }
    }
}
