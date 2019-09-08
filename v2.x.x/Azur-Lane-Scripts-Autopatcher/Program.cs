using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Azurlane
{
    internal enum Tasks
    {
        Encrypt,
        Decrypt,
        Decompile,
        Recompile,
        Unpack,
        Repack
    }

    internal static class Program
    {
        internal static int ExceptionCount;
        internal static List<Action> ListOfAction;
        internal static List<string> ListOfLua, ListOfMod;
        internal static string DirName = "CAB-android32";

        private static void Clean(string fileName)
        {
            try
            {
                if (File.Exists(PathMgr.Temp(fileName))) File.Delete(PathMgr.Temp(fileName));
                if (Directory.Exists(PathMgr.Lua(fileName).Replace($"\\{DirName}", ""))) Utils.Rmdir(PathMgr.Lua(fileName).Replace($"\\{DirName}", ""));

                foreach (var mod in ListOfMod)
                {
                    if (File.Exists(PathMgr.Temp(mod))) File.Delete(PathMgr.Temp(mod));
                    if (Directory.Exists(PathMgr.Lua(mod).Replace($"\\{DirName}", ""))) Utils.Rmdir(PathMgr.Lua(mod).Replace($"\\{DirName}", ""));
                }
            }
            catch (Exception e)
            {
                Utils.eLogger("Exception detected during cleaning", e);
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Open an AssetBundle...";
                    dialog.Filter = "Azurlane AssetBundle|scripts*";
                    dialog.CheckFileExists = true;
                    dialog.Multiselect = false;
                    dialog.ShowDialog();

                    if (File.Exists(dialog.FileName))
                    {
                        args = new[] { dialog.FileName };
                    }
                    else
                    {
                        Console.WriteLine("Please open an AssetBundle...");
                        goto END;
                    }
                }
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Invalid argument, usage: Azurlane.exe <path-to-assetbundle>");
                goto END;
            }

            var filePath = Path.GetFullPath(args[0]);
            var fileDirectoryPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine(Directory.Exists(fileDirectoryPath) ? string.Format("{0} is a directory, please input a file...", args[0]) : string.Format("{0} does not exists...", args[0]));
                goto END;
            }

            if (!AssetBundleMgr.IsAssetBundleValid(filePath))
            {
                Console.WriteLine("Not a valid AssetBundle file...");
                goto END;
            }

            ConfigMgr.Initialize();

            for (var i = 0; i < ListOfMod.Count; i++)
            {
                ListOfMod[i] = string.Format("{0}-{1}", fileName, ListOfMod[i]);
            }

            if (File.Exists(PathMgr.Local("Logs.txt")))
                File.Delete(PathMgr.Local("Logs.txt"));

            Clean(fileName);

            if (!Directory.Exists(PathMgr.Temp()))
                Directory.CreateDirectory(PathMgr.Temp());

            var index = 1;
            if (ListOfAction == null)
            {
                ListOfAction = new List<Action>()
                {
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Copying AssetBundle to temporary workspace...");
                                File.Copy(filePath, PathMgr.Temp(fileName), true);
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during copying AssetBundle to temporary workspace", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Decrypting AssetBundle...");
                                Utils.NewCommand(string.Format("Azcli.exe --decrypt \"{0}\"", PathMgr.Temp(fileName)));
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during decrypting AssetBundle", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Unpacking AssetBundle...");
                                Utils.NewCommand(string.Format("Azcli.exe --unpack \"{0}\"", PathMgr.Temp(fileName)));
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during unpacking AssetBundle", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                var showDoneMessage = true;
                                Utils.pInfof("Decrypting Lua...");
                                foreach (var lua in ListOfLua) {
                                    Utils.NewCommand(string.Format("Azcli.exe --unlock \"{0}\"", PathMgr.Lua(fileName, lua)));
                                    if (LuaMgr.CheckLuaState(PathMgr.Lua(fileName, lua)) == LuaMgr.State.Encrypted) {
                                        Utils.pDebugln(string.Format("Failed to decrypt {0}", Path.GetFileName(lua)));
                                        showDoneMessage = false;
                                    }
                                }
                                if (showDoneMessage)
                                    Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during decrypting Lua", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Decompiling Lua...");
                                foreach (var lua in ListOfLua) {
                                    Console.Write($" {index}/{ListOfLua.Count}");
                                    Utils.NewCommand(string.Format("Azcli.exe --decompile \"{0}\"", PathMgr.Lua(fileName, lua)));
                                    index++;
                                }
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during decompiling Lua", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Creating a copy of Lua & AssetBundle");
                                foreach (var mod in ListOfMod)
                                {
                                    if (!Directory.Exists(PathMgr.Lua(mod)))
                                        Directory.CreateDirectory(PathMgr.Lua(mod));

                                    foreach (var lua in ListOfLua)
                                        File.Copy(PathMgr.Lua(fileName, lua), PathMgr.Lua(mod, lua), true);
                                    File.Copy(PathMgr.Temp(fileName), PathMgr.Temp(mod), true);
                                }
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during creating a copy of Lua & AssetBundle", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Cleaning...");
                                if (File.Exists(PathMgr.Temp(fileName))) File.Delete(PathMgr.Temp(fileName));
                                if (Directory.Exists(PathMgr.Lua(fileName).Replace($"\\{DirName}", ""))) Utils.Rmdir(PathMgr.Lua(fileName).Replace($"\\{DirName}", ""));
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during cleaning", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Rewriting Lua...");
                                Utils.NewCommand("Rewriter.exe");
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during rewriting Lua", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Recompiling Lua...");
                                foreach (var mod in ListOfMod)
                                {
                                    foreach (var lua in ListOfLua)
                                        Utils.NewCommand(string.Format("Azcli.exe --recompile \"{0}\"", PathMgr.Lua(mod, lua)));
                                }
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during recompiling Lua", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                var showDoneMessage = true;
                                Utils.pInfof("Encrypting Lua...");
                                foreach (var mod in ListOfMod)
                                {
                                    foreach (var lua in ListOfLua) {
                                        Utils.NewCommand(string.Format("Azcli.exe --lock \"{0}\"", PathMgr.Lua(mod, lua)));
                                        if (LuaMgr.CheckLuaState(PathMgr.Lua(mod, lua)) == LuaMgr.State.Decrypted) {
                                            Utils.pDebugln(string.Format("Failed to encrypt {0}/{1}...", mod, Path.GetFileName(lua)));
                                            showDoneMessage = false;
                                        }
                                    }
                                }
                                if (showDoneMessage)
                                    Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during encrypting Lua", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Repacking AssetBundle...");
                                foreach (var mod in ListOfMod)
                                {
                                    Console.Write($" {index}/{ListOfMod.Count}");
                                    Utils.NewCommand(string.Format("Azcli.exe --repack \"{0}\"", PathMgr.Temp(mod)));
                                    index++;
                                }
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during repacking AssetBundle", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Encrypting AssetBundle...");
                                foreach (var mod in ListOfMod)
                                    Utils.NewCommand(string.Format("Azcli.exe --encrypt \"{0}\"", PathMgr.Temp(mod)));
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during encrypting AssetBundle", e);
                            }
                        }
                    },
                    {
                        () =>
                        {
                            try {
                                Utils.pInfof("Copying modified AssetBundle to original location...");
                                foreach (var mod in ListOfMod)
                                {
                                    if (File.Exists(Path.Combine(fileDirectoryPath, mod)))
                                        File.Delete(Path.Combine(fileDirectoryPath, mod));

                                    File.Copy(PathMgr.Temp(mod), Path.Combine(fileDirectoryPath, mod));
                                }
                                Console.Write(" <Done>\n");
                            }
                            catch (Exception e)
                            {
                                Utils.eLogger("Exception detected during copying modified AssetBundle to original location", e);
                            }
                        }
                    }
                };
            }

            try
            {
                foreach (var action in ListOfAction)
                {
                    if (index != 1)
                        index = 1;

                    action.Invoke();
                }
            }
            finally
            {
                Utils.pInfoln("Finishing...");
                Clean(fileName);

                Console.WriteLine();
                Console.WriteLine("Finished.");
            }

            END:
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}