using System.IO;
using UnityEngine;

namespace Loader
{
    internal class PathMgr
    {
        internal static string Data() => Application.persistentDataPath;

        internal static string Data(string s) => Path.Combine(Data(), s);

        internal static string Web() => "https://al.muhsekrit.club";

        internal static string Web(string s) => Path.Combine(Web(), s);

        internal static string Web(string s1, string s2) => Path.Combine(Web(s1), s2);
    }
}