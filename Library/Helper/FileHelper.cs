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

        private static string playerPath { get; } = Directory.GetCurrentDirectory() + "/player.dat";

        public static async Task<string[]> loadLines(string path) => await File.ReadAllLinesAsync(path, encoding);

        public static async Task<string> load(string path) => await File.ReadAllTextAsync(path, encoding);

        public static async Task<T> load<T>(string path) => (await load(path)).fromJson<T>();

        public static async Task save(string path, object o, bool isIndented = false) => await File.WriteAllTextAsync(path, o.toJson(isIndented), encoding);

        public static async Task<Wording> loadCharset(string charset = "zh-tw")
        {
            var lines = (await loadLines("charset/system.dat")).Union(await loadLines("charset/zh-tw.dat"));

            return new Wording(charset, lines.Where(o => !o.StartsWith("#") && !string.IsNullOrWhiteSpace(o)).Select(o =>
            {
                var line = o.Split('=');
                return new KeyValuePair<string, string>(line[0], line[1]);
            }).ToDictionary(o => o.Key, o => o.Value));
        }

        public static async Task<T> loadOption<T>() => await load<T>(optionPath);

        public static async Task saveOption(object o) => await save(optionPath, o, true);

        public static async Task<T> loadPlayer<T>() => await load<T>(playerPath);

        public static async Task savePlayer(object o) => await save(playerPath, o, true);
    }
}
