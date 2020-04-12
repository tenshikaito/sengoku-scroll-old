using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                using(var s = File.OpenRead(filepath))
                {
                    return images[filepath] = Image.FromStream(s);
                }
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
