using Client.Model;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helper
{
    public static class FileHelper
    {
        private static string userPath { get; } = Directory.GetCurrentDirectory() + "/user.dat";

        public static async Task saveUserInfo(IEnumerable<UserInfo> user) => await File.WriteAllTextAsync(userPath, user.toJson());

        public static async Task<List<UserInfo>> loadUserInfo() => (await File.ReadAllTextAsync(userPath)).fromJson<List<UserInfo>>();
    }
}
