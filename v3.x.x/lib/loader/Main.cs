using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Loader
{
    public class Main
    {
        private static object _instance;

        public static IEnumerator Initialize(Action callback)
        {
            const string fileName = "Konpaku.dll";
            var localAssemblyPath = PathMgr.Data(fileName);
            var webAssemblyPath = PathMgr.Web("v1.0", fileName);
            var isCached = false;

            yield return HttpClient.GetHeader(webAssemblyPath, "Content-Length", delegate (bool isErr, string err, string length)
            {
                if (!isErr && File.Exists(localAssemblyPath) && new FileInfo(localAssemblyPath).Length.ToString() == length)
                    isCached = true;
            });

            if (!isCached)
            {
                yield return HttpClient.GetBytes(webAssemblyPath, delegate (bool isErr, string err, byte[] bytes)
                {
                    if (!isErr)
                        File.WriteAllBytes(localAssemblyPath, bytes);
                });
            }

            if (_instance == null)
            {
                var bytes = File.ReadAllBytes(localAssemblyPath);
                var assembly = Assembly.Load(bytes);
                var type = assembly.GetType("Konpaku.Main");
                _instance = Activator.CreateInstance(type);
            }

            var method = _instance.GetType().GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
            yield return method.Invoke(_instance, new object[] { callback, "v1.0" });
        }
    }
}