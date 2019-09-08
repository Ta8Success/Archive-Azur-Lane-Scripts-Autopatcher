using System;
using System.IO;
using System.Reflection;

namespace Azurlane
{
    internal static class AssetBundleMgr
    {
        private static readonly byte[] Decrypted, Encrypted;
        private static readonly object Instance;

        static AssetBundleMgr()
        {
            if (Decrypted == null)
            {
                Decrypted = new byte[] {
                    0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53, 0x00,
                    0x00, 0x00, 0x00, 0x06, 0x35, 0x2E, 0x78, 0x2E
                };
            }

            if (Encrypted == null)
            {
                Encrypted = new byte[] {
                    0xC7, 0xD5, 0xFC, 0x1F, 0x4C, 0x92, 0x94, 0x55,
                    0x85, 0x03, 0x16, 0xA3, 0x7F, 0x7B, 0x8B, 0x55
                };
            }

            if (Instance == null)
            {
                var assembly = Assembly.Load(Properties.Resources.Salt);
                Instance = Activator.CreateInstance(assembly.GetType("LL.Salt"));
            }
        }

        internal static bool Compare(byte[] b1, byte[] b2)
        {
            try
            {
                for (var i = 0; i < b2.Length; i++)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
            }
            catch (Exception e)
            {
                Utils.eLogger("Exception detected during comparing bytes", e);
            }
            return true;
        }

        internal static void CheckAndExecute(string path, Tasks task)
        {
            var bytes = File.ReadAllBytes(path);

            if (Compare(bytes, Encrypted))
            {
                if (task == Tasks.Encrypt)
                {
                    Utils.pInfoln("AssetBundle is already encrypted... <Aborted>");
                    return;
                }
                else if (task == Tasks.Unpack || task == Tasks.Repack)
                {
                    Execute(bytes, path, Tasks.Decrypt);
                }
            }
            else if (Compare(bytes, Decrypted))
            {
                if (task == Tasks.Decrypt)
                {
                    Utils.pInfoln("AssetBundle is already decrypted... <Aborted>");
                    return;
                }
            }
            else
            {
                Utils.pInfoln("Not a valid or damaged AssetBundle file... <Aborted>");
                return;
            }

            if (task == Tasks.Decrypt || task == Tasks.Encrypt)
            {
                Execute(bytes, path, task);
            }
            else if (task == Tasks.Unpack || task == Tasks.Repack)
            {
                Execute(path, task);
            }
            Program.isInvalid = false;
        }

        private static void Execute(byte[] bytes, string path, Tasks task)
        {
            Utils.pInfof(string.Format("{0} {1}...", task == Tasks.Decrypt ? "Decrypting" : "Encrypting", Path.GetFileName(path)));
            var method = Instance.GetType().GetMethod("Make", BindingFlags.Static | BindingFlags.Public);
            bytes = (byte[])method.Invoke(Instance, new object[] { bytes, task == Tasks.Encrypt });
            File.WriteAllBytes(path, bytes);
            Console.Write(" <Done>\n");
        }

        private static void Execute(string path, Tasks task)
        {
            Utils.pInfof(string.Format("{0} {1}...", task == Tasks.Unpack ? "Unpacking" : "Repacking", Path.GetFileName(path)));
            Utils.NewCommand(string.Format("UnityEX.exe {0} \"{1}\"", task == Tasks.Unpack ? "export" : "import", path));
            Console.Write(" <Done>\n");
        }
    }
}