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

        public static void saveUserInfo(IEnumerable<UserInfo> user) => File.WriteAllText(userPath, user.toJson());

        public static List<UserInfo> loadUserInfo() => File.ReadAllText(userPath).fromJson<List<UserInfo>>();
    }
}
