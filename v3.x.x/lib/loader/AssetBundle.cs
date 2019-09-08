using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Loader
{
    public class AssetBundle
    {
        private static object _instance;

        public static byte[] Initialize(TextAsset textAsset, string fileName)
        {
            var localAssemblyPath = PathMgr.Data("Konpaku.dll");

            if (_instance == null)
            {
                var bytes = File.ReadAllBytes(localAssemblyPath);
                var assembly = Assembly.Load(bytes);
                var type = assembly.GetType("Konpaku.AssetBundle");
                _instance = Activator.CreateInstance(type);
            }

            var method = _instance.GetType().GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
            return (byte[])method.Invoke(_instance, new object[] { textAsset, fileName });
        }
    }
}