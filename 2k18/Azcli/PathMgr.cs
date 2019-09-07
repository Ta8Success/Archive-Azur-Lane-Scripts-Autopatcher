using System.IO;
using System.Reflection;

namespace Azurlane
{
    internal static class PathMgr
    {
        internal static string Local(string path = null)
        {
            if (path != null && !File.Exists(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path == null ? Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) : Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
        }

        internal static string Thirdparty(string path = null) => path != null ? Path.Combine(Local(ConfigMgr.ThirdpartyFolder), path) : Local(ConfigMgr.ThirdpartyFolder);
    }
}