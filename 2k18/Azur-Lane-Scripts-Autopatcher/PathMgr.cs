using System.IO;
using System.Reflection;

namespace Azurlane
{
    internal static class PathMgr
    {
        internal static string Local(string path = null) => path != null ? Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path) : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        internal static string Lua(string name) => Path.Combine(Temp("Unity_Assets_Files"), string.Format("{0}\\CAB-ios64", name));

        internal static string Lua(string name, string lua) => Path.Combine(Lua(name), lua);

        internal static string Temp(string path = null) => path != null ? Path.Combine(Local(ConfigMgr.TemporaryFolder), path) : Local(ConfigMgr.TemporaryFolder);
    }
}