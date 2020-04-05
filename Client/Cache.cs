using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Client
{
    public class Cache : IDisposable
    {
        private Dictionary<string, Image> images = new Dictionary<string, Image>();

        public Image getImage(string filepath)
        {
            if (!images.TryGetValue(filepath, out var img))
            {
                return images[filepath] = Image.FromFile(filepath);
            }

            return img;
        }

        public void Dispose()
        {
            foreach (var img in images) img.Value.Dispose();

            images.Clear();
        }
    }
}
