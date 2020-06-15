using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public static class FileHelper
    {
        public static readonly Encoding encoding = Encoding.UTF8;

        private static string optionPath { get; } = Directory.GetCurrentDirectory() + "/option.json";

        private static string userPath { get; } = Directory.GetCurrentDirectory() + "/user.dat";

        public static string[] loadLines(string path) => File.ReadAllLines(path, encoding);

        public static string load(string path) => File.ReadAllText(path, encoding);

        public static T load<T>(string path) => load(path).fromJson<T>();

        public static void save(string path, object o, bool isIndented = false) => File.WriteAllText(path, o.toJson(isIndented), encoding);

        public static T loadOption<T>() => load<T>(optionPath);

        public static void saveOption(object o) => save(optionPath, o, true);

        public static T loadUser<T>() => load<T>(userPath);

        public static void saveUser(object o) => save(userPath, o, true);
    }
}
