using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class TreeObject<T> where T : TreeObject<T>, IDisposable
    {
        protected List<T> children = new List<T>();

        public T parent { get; private set; }

        public bool isRoot => parent == null;

        public TreeObject() { }

        public TreeObject(T parent) => moveTo(parent);

        public void moveTo(T go)
        {
            if (go == null) throw new ArgumentNullException();

            if (!isRoot) parent.children.Remove((T)this);

            parent = go;

            parent.children.Add((T)this);
        }

        public virtual void Dispose()
        {
            parent = null;

            children.ForEach(o => Dispose());
            children.Clear();
        }
    }
}
