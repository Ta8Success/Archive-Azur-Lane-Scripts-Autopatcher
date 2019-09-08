using System.IO;

namespace Azurlane
{
    internal class AssetBundleMgr
    {
        private static readonly byte[] Decrypted, Encrypted;

        static AssetBundleMgr()
        {
            if (Encrypted == null)
            {
                Encrypted = new byte[] {
                    0xC7, 0xD5, 0xFC, 0x1F, 0x4C, 0x92, 0x94, 0x55,
                    0x85, 0x03, 0x16, 0xA3, 0x7F, 0x7B, 0x8B, 0x55
                };
            }
        }

        internal static bool IsAssetBundleValid(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Compare(bytes, Encrypted);
        }

        private static bool Compare(byte[] b1, byte[] b2)
        {
            for (var i = 0; i < b2.Length; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }
    }
}