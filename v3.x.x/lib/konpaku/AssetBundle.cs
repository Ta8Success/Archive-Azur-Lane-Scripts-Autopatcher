using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Konpaku
{
    public class AssetBundle
    {
        private static List<string> _keys;

        public static byte[] Initialize(TextAsset textAsset, string fileName)
        {
            if (Main.SelectedMod == 0)
                return textAsset.bytes;

            if (_keys == null)
                _keys = new List<string>();

            AddKey("aircraft_template");
            AddKey("enemy_data_skill");
            AddKey("enemy_data_statistics");

            if (Main.ReplaceSkin) AddKey("ship_data_statistics");
            if (Main.SelectedMod.ToString().ToLower().Contains("damage")) AddKey("weapon_property");

            foreach (var key in _keys)
            {
                if (!fileName.Contains(key))
                    continue;

                foreach (var filePath in Directory.GetFiles(PathMgr.Raw(Main.SelectedMod.ToString().ToLower().Replace("_", "-")), "*.*", SearchOption.AllDirectories))
                {
                    if (!filePath.Contains(key))
                        continue;

                    var bytes = File.ReadAllBytes(filePath);
                    return bytes;
                }
            }

            return textAsset.bytes;
        }

        private static void AddKey(string value) => _keys.Add(value);
    }
}