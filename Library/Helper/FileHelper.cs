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

        public static async Task<string[]> loadLines(string path) => await File.ReadAllLinesAsync(path, encoding);

        public static async Task<string> load(string path) => await File.ReadAllTextAsync(path, encoding);

        public static async Task<T> load<T>(string path) => (await load(path)).fromJson<T>();

        public static async Task save(string path, object o, bool isIndented = false) => await File.WriteAllTextAsync(path, o.toJson(isIndented), encoding);

        public static async Task<T> loadOption<T>() => await load<T>(optionPath);

        public static async Task saveOption(object o) => await save(optionPath, o, true);

        public static async Task<T> loadUser<T>() => await load<T>(userPath);

        public static async Task saveUser(object o) => await save(userPath, o, true);
    }
}
