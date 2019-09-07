using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azurlane
{
    internal enum Tasks
    {
        None,
        Encrypt,
        Decrypt,
        Decompile,
        Recompile,
        Unpack,
        Repack
    }

    internal static class Program
    {
        internal static bool isInvalid = true;

        private static readonly List<string> ListOfAssetBundle = new List<string>(), ListOfLua = new List<string>();
        private static readonly Dictionary<string, List<string>> Parameters = new Dictionary<string, List<string>>();
        private static string CurrentOption;

        internal static void Main(string[] args)
        {
            var _showHelp = args.Length < 2;
            var options = new OptionSet()
            {
                {"u|unlock", "Decrypt Lua", v => CurrentOption = "lua.unlock"},
                {"l|lock", "Encrypt Lua", v => CurrentOption = "lua.lock"},
                {"d|decompile", "Decompile Lua (will automatically decrypt if Lua is encrypted)", v => CurrentOption = "lua.decompile"},
                {"r|recompile", "Recompile Lua", v => CurrentOption = "lua.recompile"},
                {"decrypt", "Decrypt AssetBundle",  v => CurrentOption = "assetbundle.decrypt"},
                {"encrypt", "Encrypt AssetBundle", v => CurrentOption = "assetbundle.encrypt"},
                {"unpack", "Unpack all lua from AssetBundle (will automatically decrypt if AssetBundle is encrypted)", v => CurrentOption = "assetbundle.unpack"},
                {"repack", "Repack all lua from AssetBundle", v => CurrentOption = "assetbundle.repack"},
                {"<>", v => {
                    if (CurrentOption == null) {
                        _showHelp = true;
                        return;
                    }

                    List<string> values;
                    if (Parameters.TryGetValue(CurrentOption, out values))
                    {
                        values.Add(v);
                    }
                    else
                    {
                        if (values == null)
                            values = new List<string> { v };
                        Parameters.Add(CurrentOption, values);
                    }
                }}
            };

            if (_showHelp)
            {
                return;
            }
            else
            {
                try
                {
                    options.Parse(args);
                }
                catch
                {
                    // Empty
                }
            }

            foreach (var parameter in Parameters)
            {
                foreach (var value in parameter.Value)
                {
                    if (!File.Exists(value) && !Directory.Exists(value))
                    {
                        Console.WriteLine(string.Format("A file or directory named {0} does not exists.", value));
                    }
                    else if (File.Exists(value))
                    {
                        if (parameter.Key.Contains("lua.")) ListOfLua.Add(Path.GetFullPath(value));
                        else ListOfAssetBundle.Add(Path.GetFullPath(value));
                    }
                    else if (Directory.Exists(value))
                    {
                        if (parameter.Key.Contains("lua."))
                        {
                            foreach (var file in Directory.GetFiles(Path.GetFullPath(value), "*.lua*", SearchOption.AllDirectories))
                                ListOfLua.Add(file);
                        }
                        else
                        {
                            foreach (var file in Directory.GetFiles(Path.GetFullPath(value), "*", SearchOption.AllDirectories))
                                ListOfAssetBundle.Add(file);
                        }
                    }
                }
            }

            ConfigMgr.Initialize();

            if (CurrentOption.Contains("lua."))
            {
                foreach (var lua in ListOfLua)
                    LuaMgr.CheckAndExecute(lua, CurrentOption.Contains(".unlock") ? Tasks.Decrypt : (CurrentOption.Contains(".lock") ? Tasks.Encrypt : (CurrentOption.Contains(".decompile") ? Tasks.Decompile : Tasks.Recompile)));
            }
            else if (CurrentOption.Contains("assetbundle."))
            {
                foreach (var assetbundle in ListOfAssetBundle)
                    AssetBundleMgr.CheckAndExecute(assetbundle, CurrentOption.Contains(".decrypt") ? Tasks.Decrypt : (CurrentOption.Contains(".encrypt") ? Tasks.Encrypt : (CurrentOption.Contains(".unpack") ? Tasks.Unpack : Tasks.Repack)));
            }

            if (!isInvalid && !CurrentOption.Contains(".repack") && !CurrentOption.Contains(".decrypt") && !CurrentOption.Contains(".encrypt"))
            {
                Console.WriteLine();
                Console.WriteLine(string.Format("{0} {1}is done", CurrentOption.Contains(".unlock") || CurrentOption.Contains(".decrypt") ? "Decrypt" : CurrentOption.Contains(".lock") || CurrentOption.Contains(".encrypt") ? "Encrypt" : CurrentOption.Contains(".decompile") ? "Decompile" : CurrentOption.Contains(".recompile") ? "Recompile" : CurrentOption.Contains(".unpack") ? "Unpacking" : "Repacking", CurrentOption.Contains("lua.") ? "" : "assetbundle "));
                if (!CurrentOption.Contains(".unpack"))
                    Console.WriteLine(string.Format("Success: {0} - Failed: {1}", LuaMgr.SuccessCount, LuaMgr.FailedCount));
            }
        }
    }
}