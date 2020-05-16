using Library.Helper;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameResourceProcessor
    {
        private const string DirName = "resource";
        private const string ImageDirName = "image";

        private string packageName;
        private string packagePath;
        private string packageFullPath;

        public GameResourceProcessor(string packageName = "default")
        {
            packageName = packageName ?? "default";

            this.packageName = packageName;

            packagePath = $"{DirName}/{packageName}";
            packageFullPath = $"{Directory.GetCurrentDirectory()}/{packagePath}";
        }

        public string getSystemImageFilePath(string fileName, bool isAbsolute = false)
            => isAbsolute ? $"{packagePath}/{ImageDirName}/system/{fileName}" : $"{packageFullPath}/{ImageDirName}/system/{fileName}";

        public string getTileMapImageFilePath(string fileName, bool isAbsolute = false)
            => isAbsolute ? $"{packagePath}/{ImageDirName}/tilemap/{fileName}" : $"{packageFullPath}/{ImageDirName}/tilemap/{fileName}";

    }
}
