using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azurlane
{
    internal static class ConfigMgr
    {
        internal static bool IsCreateGodMode;
        internal static bool IsCreateGodModeCooldown;
        internal static bool IsCreateGodModeDamage;
        internal static bool IsCreateGodModeDamageCooldown;
        internal static bool IsCreateGodModeDamageCooldownWeakEnemy;
        internal static bool IsCreateGodModeDamageWeakEnemy;
        internal static bool IsCreateGodModeWeakEnemy;
        internal static bool IsCreateWeakEnemy;
        internal static bool IsRemoveSkill;
        internal static string TemporaryFolder;
        internal static string Version;

        private static Dictionary<string, string> Instance;

        static ConfigMgr()
        {
            if (Program.ListOfLua == null)
            {
                Program.ListOfLua = new List<string>()
                {
                   "aircraft_template.lua.txt",
                   "enemy_data_statistics.lua.txt"
                };
            }

            if (Program.ListOfMod == null)
                Program.ListOfMod = new List<string>();

            if (Instance == null)
            {
                var iniPath = PathMgr.Local("Configuration.ini");

                if (File.Exists(iniPath))
                    Update(iniPath);

                if (!File.Exists(iniPath))
                    File.WriteAllText(iniPath, Properties.Resources.Configuration);

                Instance = new Dictionary<string, string>();
                foreach (var line in File.ReadAllLines(iniPath))
                {
                    if (line.Contains('='))
                    {
                        var s = line.Split('=');
                        if (s[0] == "Temporary_Folder" || s[0] == "Version" || s[0] == "Godmode" || s[0] == "Weakenemy" ||
                            s[0] == "Godmode_Damage" || s[0] == "Godmode_Cooldown" || s[0] == "Godmode_Weakenemy" ||
                            s[0] == "Godmode_Damage_Cooldown" || s[0] == "Godmode_Damage_Weakenemy" ||
                            s[0] == "Godmode_Damage_Cooldown_Weakenemy" || s[0] == "RemoveSkill")
                            Instance.Add(s[0], s[1]);
                    }
                }

                Parse();
            }
        }

        internal static void Initialize()
        {
            if (IsCreateGodMode)
                Program.ListOfMod.Add("godmode");

            if (IsCreateWeakEnemy)
                Program.ListOfMod.Add("weakenemy");

            if (IsCreateGodModeDamage)
                Program.ListOfMod.Add("godmode-damage");

            if (IsCreateGodModeCooldown)
                Program.ListOfMod.Add("godmode-cooldown");

            if (IsCreateGodModeWeakEnemy)
                Program.ListOfMod.Add("godmode-weakenemy");

            if (IsCreateGodModeDamageCooldown)
                Program.ListOfMod.Add("godmode-damage-cooldown");

            if (IsCreateGodModeDamageWeakEnemy)
                Program.ListOfMod.Add("godmode-damage-weakenemy");

            if (IsCreateGodModeDamageCooldownWeakEnemy)
                Program.ListOfMod.Add("godmode-damage-cooldown-weakenemy");

            if (IsCreateGodModeCooldown || IsCreateGodModeDamage || IsCreateGodModeDamageCooldown || IsCreateGodModeDamageCooldownWeakEnemy)
                Program.ListOfLua.Add("weapon_property.lua.txt");

            if (IsRemoveSkill)
                Program.ListOfLua.Add("enemy_data_skill.lua.txt");
        }

        private static bool GetBool(string key)
        {
            if (Instance[key].ToLower() != "true" || Instance[key].ToLower() == "ignore")
            {
                return false;
            }
            return true;
        }

        private static string GetString(string key) => Instance[key];

        private static void Parse()
        {
            TemporaryFolder = GetString("Temporary_Folder");
            Version = GetString("Version");
            IsCreateGodMode = GetBool("Godmode");
            IsCreateWeakEnemy = GetBool("Weakenemy");
            IsCreateGodModeDamage = GetBool("Godmode_Damage");
            IsCreateGodModeCooldown = GetBool("Godmode_Cooldown");
            IsCreateGodModeWeakEnemy = GetBool("Godmode_Weakenemy");
            IsCreateGodModeDamageCooldown = GetBool("Godmode_Damage_Cooldown");
            IsCreateGodModeDamageWeakEnemy = GetBool("Godmode_Damage_Weakenemy");
            IsCreateGodModeDamageCooldownWeakEnemy = GetBool("Godmode_Damage_Cooldown_Weakenemy");
            IsRemoveSkill = GetBool("RemoveSkill");
        }

        private static void Update(string path)
        {
            if (!File.ReadAllText(path).Contains("2.8.2"))
            {
                File.Copy(path, string.Concat(Path.GetFileNameWithoutExtension(path), ".old", Path.GetExtension(path)), true);
                File.Delete(path);
            }
        }
    }
}