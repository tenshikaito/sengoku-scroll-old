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
    public class GameResourceManager
    {
        private const string DirName = "resource";
        private const string ImageDirName = "image";

        private string packageName;
        private string packagePath;
        private string packageFullPath;

        public GameResourceManager(string packageName = "default")
        {
            this.packageName = packageName = packageName ?? "default";

            packagePath = $"{DirName}/{packageName}";
            packageFullPath = $"{Directory.GetCurrentDirectory()}/{packagePath}";
        }

        public string getSystemImageFilePath(string fileName) => $"{packagePath}/{ImageDirName}/system/{fileName}";

        public string getSystemImageFileAbsolutePath(string fileName) => $"{packageFullPath}/{ImageDirName}/system/{fileName}";

        public string getTileMapImageFilePath(string fileName) => $"{packagePath}/{ImageDirName}/tilemap/{fileName}";

        public string getTileMapImageFileAbsolutePath(string fileName) => $"{packageFullPath}/{ImageDirName}/tilemap/{fileName}";
    }
}
