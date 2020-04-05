using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public class StateManager<Status> : IInputEventListener where Status : GameStatusAdapter
    {
        public Status currentStatus { get; protected set; }

        public virtual void switchStatus(Status s)
        {
            if (currentStatus?.GetType().Name == s.GetType().Name) return;

            currentStatus?.finish();
            currentStatus = s;
            currentStatus.start();
        }

        public void update()
        {
            currentStatus.update();
        }

        public void draw()
        {
            currentStatus.draw();
        }

        public virtual void mouseMoved(MouseEventArgs e)
        {
            currentStatus.mouseMoved(e);
        }

        public virtual void mouseDragging(MouseEventArgs e, Point p)
        {
            currentStatus.mouseDragging(e, p);
        }

        public virtual void mouseEntered(MouseEventArgs e)
        {
            currentStatus.mouseEntered(e);
        }

        public virtual void mouseExited(MouseEventArgs e)
        {
            currentStatus.mouseExited(e);
        }

        public virtual void mouseClicked(MouseEventArgs e)
        {
            currentStatus.mouseClicked(e);
        }

        public virtual void mousePressed(MouseEventArgs e)
        {
            currentStatus.mousePressed(e);
        }

        public virtual void mouseReleased(MouseEventArgs e)
        {
            currentStatus.mouseReleased(e);
        }

        public virtual void mouseWheelScrolled(MouseEventArgs e)
        {
            currentStatus.mouseWheelScrolled(e);
        }
    }

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
    }
}
