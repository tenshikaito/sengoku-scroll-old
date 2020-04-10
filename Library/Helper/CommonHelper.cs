using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Helper
{
    public static class CommonHelper
    {
        public static string toJson(this object o, bool isIndented = false) => JsonConvert.SerializeObject(o, isIndented ? Formatting.Indented : Formatting.None);

        public static T fromJson<T>(this string s) => JsonConvert.DeserializeObject<T>(s);

        public static List<T> getEnumList<T>() where T : struct => ((T[])Enum.GetValues(typeof(T))).Cast<T>().ToList();
    }
}
