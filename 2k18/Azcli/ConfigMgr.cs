using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azurlane
{
    internal static class ConfigMgr
    {
        internal static string ThirdpartyFolder;

        private static Dictionary<string, string> Instance;

        static ConfigMgr()
        {
            if (Instance == null)
            {
                var iniPath = PathMgr.Local("Configuration.ini");

                Instance = new Dictionary<string, string>();
                foreach (var line in File.ReadAllLines(iniPath))
                {
                    if (line.Contains('='))
                    {
                        var s = line.Split('=');
                        if (s[0] == "Thirdparty_Folder")
                            Instance.Add(s[0], s[1]);
                    }
                }
            }
        }

        internal static void Initialize() => ThirdpartyFolder = GetString("Thirdparty_Folder");

        private static bool GetBool(string key)
        {
            if (Instance[key].ToLower() != "true" || Instance[key].ToLower() == "ignore")
            {
                return false;
            }
            return true;
        }

        private static string GetString(string key) => Instance[key];
    }
}