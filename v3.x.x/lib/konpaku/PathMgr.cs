using System.IO;
using UnityEngine;

namespace Konpaku
{
    internal class PathMgr
    {
        internal static string Data() => Application.persistentDataPath;

        internal static string Data(string s) => Path.Combine(Data(), s);

        internal static string Raw() => WorkingDirectory("raw");

        internal static string Raw(string s) => Path.Combine(Raw(), s);

        internal static string Web() => "https://al.muhsekrit.club";

        internal static string Web(string s) => Path.Combine(Web(), s);

        internal static string Web(string s1, string s2) => Path.Combine(Web(s1), s2);

        internal static string Web(string s1, string s2, string s3) => Path.Combine(Web(s1, s2), s3);

        internal static string WorkingDirectory() => Data("Other");

        internal static string WorkingDirectory(string s) => Path.Combine(WorkingDirectory(), s);

        internal static string WorkingDirectory(string s1, string s2) => Path.Combine(WorkingDirectory(s1), s2);
    }
}