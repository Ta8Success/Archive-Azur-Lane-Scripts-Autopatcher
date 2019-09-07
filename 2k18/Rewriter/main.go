package main

import (
	"archive/zip"
	"io"
	"io/ioutil"
	"os"
	"path/filepath"
	"regexp"
	"strings"

	"github.com/go-ini/ini"
)

type Configuration struct {
	Ini     *ini.File
	Version string
	Debug   bool
	Path    struct {
		Temporary_Folder,
		Thirdparty_Folder string
	}
	Mod struct {
		Godmode,
		Weakenemy,
		Godmode_Damage,
		Godmode_Cooldown,
		Godmode_Weakenemy,
		Godmode_Damage_Cooldown,
		Godmode_Damage_Weakenemy,
		Godmode_Damage_Cooldown_Weakenemy bool
	}
	Aircraft struct {
		Hp,
		HpGrowth,
		Accuracy,
		AccuracyGrowth,
		AttackPower,
		AttackPowerGrowth,
		CrashDamage,
		Speed string
	}
	Weapon struct {
		Damage,
		ReloadMax string
	}
	Enemy struct {
		AntiAir,
		AntiAirGrowth,
		AntiSubmarine,
		Armor,
		ArmorGrowth,
		Cannon,
		CannonGrowth,
		Evasion,
		EvasionGrowth,
		Hit,
		HitGrowth,
		Hp,
		HpGrowth,
		Luck,
		LuckGrowth,
		Reload,
		ReloadGrowth,
		Speed,
		SpeedGrowth,
		Torpedo,
		TorpedoGrowth string
		DisarmWeapon,
		RemoveSkill bool
	}
	Extra struct {
		Hp string
	}
}

func main() {
	config := new(Configuration)
	config.Load("Configuration.ini")

	var files string

	filepath.Walk(config.Path.Temporary_Folder, func(filePath string, fileInfo os.FileInfo, e error) error {
		if fileInfo.Mode().IsRegular() {
			if strings.Contains(fileInfo.Name(), ".") && config.Debug {
				if !strings.Contains(files, filePath) {
					files += filePath + ":"
				}
			}
			file, err := ioutil.ReadFile(filePath)
			if err != nil {
				return err
			}
			contents := string(file)
			if strings.Contains(fileInfo.Name(), "aircraft_template") && strings.Contains(filePath, "godmode") {
				if config.Aircraft.Hp != "ignore" {
					contents = Rewrite(contents, `(max_hp =) .*(,)`, `$1 `+config.Aircraft.Hp+`$2`)
				}
				if config.Aircraft.HpGrowth != "ignore" {
					contents = Rewrite(contents, `(hp_growth =) .*(,)`, `$1 `+config.Aircraft.HpGrowth+`$2`)
				}
				if config.Aircraft.Accuracy != "ignore" {
					contents = Rewrite(contents, `(accuracy =) .*(,)`, `$1 `+config.Aircraft.Accuracy+`$2`)
				}
				if config.Aircraft.AccuracyGrowth != "ignore" {
					contents = Rewrite(contents, `(ACC_growth =) .*(,)`, `$1 `+config.Aircraft.AccuracyGrowth+`$2`)
				}
				if config.Aircraft.AttackPower != "ignore" {
					contents = Rewrite(contents, `(attack_power =) .*(,)`, `$1 `+config.Aircraft.AttackPower+`$2`)
				}
				if config.Aircraft.AttackPowerGrowth != "ignore" {
					contents = Rewrite(contents, `(AP_growth =) .*(,)`, `$1 `+config.Aircraft.AttackPowerGrowth+`$2`)
				}
				if config.Aircraft.CrashDamage != "ignore" {
					contents = Rewrite(contents, `(crash_DMG =) .*(,)`, `$1 `+config.Aircraft.CrashDamage+`$2`)
				}
				if config.Aircraft.Speed != "ignore" {
					contents = Rewrite(contents, `(speed =) .*(,)`, `$1 `+config.Aircraft.Speed+`$2`)
				}
			}
			if strings.Contains(fileInfo.Name(), "weapon_property") {
				if strings.Contains(filePath, "damage") && config.Weapon.Damage != "ignore" {
					contents = Rewrite(contents, `(damage =) .*(,)`, `$1 `+config.Weapon.Damage+`$2`)
				}
				if strings.Contains(filePath, "cooldown") && config.Weapon.ReloadMax != "ignore" {
					contents = Rewrite(contents, `(reload_max =) .*(,)`, `$1 `+config.Weapon.ReloadMax+`$2`)
				}
			}
			if strings.Contains(fileInfo.Name(), "enemy_data_statistics") {
				if strings.Contains(filePath, "godmode") && config.Enemy.DisarmWeapon {
					contents = Rewrite(contents, `(equipment_list =) ({)[^}]+(})`, `$1$2$3`)
				}
				if strings.Contains(filePath, "weakenemy") {
					if config.Enemy.Hp != "ignore" {
						contents = Rewrite(contents, `(durability =) .*(,)`, `$1 `+config.Enemy.Hp+`$2`)
					}
					if config.Enemy.HpGrowth != "ignore" {
						contents = Rewrite(contents, `(durability_growth =) .*(,)`, `$1 `+config.Enemy.HpGrowth+`$2`)
					}
				}
				if config.Enemy.AntiAir != "ignore" {
					contents = Rewrite(contents, `(antiaircraft =) .*(,)`, `$1 `+config.Enemy.AntiAir+`$2`)
				}
				if config.Enemy.AntiAirGrowth != "ignore" {
					contents = Rewrite(contents, `(antiaircraft_growth =) .*(,)`, `$1 `+config.Enemy.AntiAirGrowth+`$2`)
				}
				if config.Enemy.AntiSubmarine != "ignore" {
					contents = Rewrite(contents, `(antisub =) .*(,)`, `$1 `+config.Enemy.AntiSubmarine+`$2`)
				}
				if config.Enemy.Armor != "ignore" {
					contents = Rewrite(contents, `(armor =) .*(,)`, `$1 `+config.Enemy.Armor+`$2`)
				}
				if config.Enemy.ArmorGrowth != "ignore" {
					contents = Rewrite(contents, `(armor_growth =) .*(,)`, `$1 `+config.Enemy.ArmorGrowth+`$2`)
				}
				if config.Enemy.Cannon != "ignore" {
					contents = Rewrite(contents, `(cannon =) .*(,)`, `$1 `+config.Enemy.Cannon+`$2`)
				}
				if config.Enemy.CannonGrowth != "ignore" {
					contents = Rewrite(contents, `(cannon_growth =) .*(,)`, `$1 `+config.Enemy.CannonGrowth+`$2`)
				}
				if config.Enemy.Evasion != "ignore" {
					contents = Rewrite(contents, `(dodge =) .*(,)`, `$1 `+config.Enemy.Evasion+`$2`)
				}
				if config.Enemy.EvasionGrowth != "ignore" {
					contents = Rewrite(contents, `(dodge_growth =) .*(,)`, `$1 `+config.Enemy.EvasionGrowth+`$2`)
				}
				if config.Enemy.Hit != "ignore" {
					contents = Rewrite(contents, `(hit =) .*(,)`, `$1 `+config.Enemy.Hit+`$2`)
				}
				if config.Enemy.HitGrowth != "ignore" {
					contents = Rewrite(contents, `(hit_growth =) .*(,)`, `$1 `+config.Enemy.HitGrowth+`$2`)
				}
				if config.Enemy.Luck != "ignore" {
					contents = Rewrite(contents, `(luck =) .*(,)`, `$1 `+config.Enemy.Luck+`$2`)
				}
				if config.Enemy.LuckGrowth != "ignore" {
					contents = Rewrite(contents, `(luck_growth =) .*(,)`, `$1 `+config.Enemy.LuckGrowth+`$2`)
				}
				if config.Enemy.Reload != "ignore" {
					contents = Rewrite(contents, `(reload =) .*(,)`, `$1 `+config.Enemy.Reload+`$2`)
				}
				if config.Enemy.ReloadGrowth != "ignore" {
					contents = Rewrite(contents, `(reload_growth =) .*(,)`, `$1 `+config.Enemy.ReloadGrowth+`$2`)
				}
				if config.Enemy.Speed != "ignore" {
					contents = Rewrite(contents, `(speed =) .*(,)`, `$1 `+config.Enemy.Speed+`$2`)
				}
				if config.Enemy.SpeedGrowth != "ignore" {
					contents = Rewrite(contents, `(speed_growth =) .*(,)`, `$1 `+config.Enemy.SpeedGrowth+`$2`)
				}
				if config.Enemy.Torpedo != "ignore" {
					contents = Rewrite(contents, `(torpedo =) .*(,)`, `$1 `+config.Enemy.Torpedo+`$2`)
				}
				if config.Enemy.TorpedoGrowth != "ignore" {
					contents = Rewrite(contents, `(torpedo_growth =) .*(,)`, `$1 `+config.Enemy.TorpedoGrowth+`$2`)
				}
			}
			if strings.Contains(fileInfo.Name(), "enemy_data_skill") && config.Enemy.RemoveSkill {
				contents = Rewrite(contents, `is_repeat = 1`, `is_repeat = 0`)
				contents = Rewrite(contents, `(skill_list =) ({)[^}]+(})`, `$1$2$3`)
			}
			if strings.Contains(fileInfo.Name(), "extraenemy_template") && config.Extra.Hp != "ignore" {
				contents = Rewrite(contents, `(hp =) .*(,)`, `$1 `+config.Extra.Hp+`$2`)
			}
			ioutil.WriteFile(filePath, []byte(contents), os.ModePerm)
		}
		if config.Debug {
			if _, err := os.Stat("Debug.zip"); os.IsExist(err) {
				os.Remove("Debug.zip")
			}
			ZipFiles("Debug.zip", strings.Split(files, ":"))
		}
		return nil
	})
}

func (config *Configuration) Load(configPath string) {
	input, err := ini.Load(configPath)
	if err == nil {
		config.Ini = input
		config.Parse()
	}
}

func (config *Configuration) Parse() {
	config.Version = config.GetString("", "Version")
	config.Debug = config.GetBool("", "GenerateDebugZip")

	// [Path]
	config.Path.Temporary_Folder = config.GetString("Path", "Temporary_Folder")
	config.Path.Thirdparty_Folder = config.GetString("Path", "Thirdparty_Folder")

	// [Mod]
	config.Mod.Godmode = config.GetBool("Mod", "Godmode")
	config.Mod.Weakenemy = config.GetBool("Mod", "Weakenemy")
	config.Mod.Godmode_Damage = config.GetBool("Mod", "Godmode_Damage")
	config.Mod.Godmode_Cooldown = config.GetBool("Mod", "Godmode_Cooldown")
	config.Mod.Godmode_Weakenemy = config.GetBool("Mod", "Godmode_Weakenemy")
	config.Mod.Godmode_Damage_Cooldown = config.GetBool("Mod", "Godmode_Damage_Cooldown")
	config.Mod.Godmode_Damage_Weakenemy = config.GetBool("Mod", "Godmode_Damage_Weakenemy")
	config.Mod.Godmode_Damage_Cooldown_Weakenemy = config.GetBool("Mod", "Godmode_Damage_Cooldown_Weakenemy")

	// [Aircraft]
	config.Aircraft.Hp = config.GetString("Aircraft", "Hp")
	config.Aircraft.HpGrowth = config.GetString("Aircraft", "HpGrowth")
	config.Aircraft.Accuracy = config.GetString("Aircraft", "Accuracy")
	config.Aircraft.AccuracyGrowth = config.GetString("Aircraft", "AccuracyGrowth")
	config.Aircraft.AttackPower = config.GetString("Aircraft", "AttackPower")
	config.Aircraft.AttackPowerGrowth = config.GetString("Aircraft", "AttackPowerGrowth")
	config.Aircraft.CrashDamage = config.GetString("Aircraft", "CrashDamage")
	config.Aircraft.Speed = config.GetString("Aircraft", "Speed")

	// [Weapon]
	config.Weapon.Damage = config.GetString("Weapon", "Damage")
	config.Weapon.ReloadMax = config.GetString("Weapon", "ReloadMax")

	// [Enemy]
	config.Enemy.AntiAir = config.GetString("Enemy", "AntiAir")
	config.Enemy.AntiAirGrowth = config.GetString("Enemy", "AntiAirGrowth")
	config.Enemy.AntiSubmarine = config.GetString("Enemy", "AntiSubmarine")
	config.Enemy.Armor = config.GetString("Enemy", "Armor")
	config.Enemy.ArmorGrowth = config.GetString("Enemy", "ArmorGrowth")
	config.Enemy.Cannon = config.GetString("Enemy", "Cannon")
	config.Enemy.CannonGrowth = config.GetString("Enemy", "CannonGrowth")
	config.Enemy.Evasion = config.GetString("Enemy", "Evasion")
	config.Enemy.EvasionGrowth = config.GetString("Enemy", "EvasionGrowth")
	config.Enemy.Hit = config.GetString("Enemy", "Hit")
	config.Enemy.HitGrowth = config.GetString("Enemy", "HitGrowth")
	config.Enemy.Hp = config.GetString("Enemy", "Hp")
	config.Enemy.HpGrowth = config.GetString("Enemy", "HpGrowth")
	config.Enemy.Luck = config.GetString("Enemy", "Luck")
	config.Enemy.LuckGrowth = config.GetString("Enemy", "LuckGrowth")
	config.Enemy.Reload = config.GetString("Enemy", "Reload")
	config.Enemy.ReloadGrowth = config.GetString("Enemy", "ReloadGrowth")
	config.Enemy.Speed = config.GetString("Enemy", "Speed")
	config.Enemy.SpeedGrowth = config.GetString("Enemy", "SpeedGrowth")
	config.Enemy.Torpedo = config.GetString("Enemy", "Torpedo")
	config.Enemy.TorpedoGrowth = config.GetString("Enemy", "TorpedoGrowth")
	config.Enemy.DisarmWeapon = config.GetBool("Enemy", "DisarmWeapon")
	config.Enemy.RemoveSkill = config.GetBool("Enemy", "RemoveSkill")

	// Extra
	config.Extra.Hp = config.GetString("Extra", "Hp")
}

func (config *Configuration) GetBool(s string, k string) bool {
	value := config.Ini.Section(s).Key(k).String()
	if strings.ToLower(value) != "true" || strings.ToLower(value) == "ignore" {
		return false
	}
	return true
}

func (config *Configuration) GetString(s string, k string) string {
	value := config.Ini.Section(s).Key(k).String()
	if strings.ToLower(value) == "false" || strings.ToLower(value) == "ignore" {
		return "ignore"
	}
	return value
}

func Rewrite(content string, regex string, replacement string) string {
	return regexp.MustCompile(string(regex)).ReplaceAllString(string(content), replacement)
}

// golangcode.com/create-zip-files-in-go
func ZipFiles(filename string, files []string) error {

	newfile, err := os.Create(filename)
	if err != nil {
		return err
	}
	defer newfile.Close()

	zipWriter := zip.NewWriter(newfile)
	defer zipWriter.Close()

	// Add files to zip
	for _, file := range files {

		zipfile, err := os.Open(file)
		if err != nil {
			return err
		}
		defer zipfile.Close()

		// Get the file information
		info, err := zipfile.Stat()
		if err != nil {
			return err
		}

		header, err := zip.FileInfoHeader(info)
		if err != nil {
			return err
		}

		// Change to deflate to gain better compression
		// see http://golang.org/pkg/archive/zip/#pkg-constants
		header.Method = zip.Deflate

		writer, err := zipWriter.CreateHeader(header)
		if err != nil {
			return err
		}
		_, err = io.Copy(writer, zipfile)
		if err != nil {
			return err
		}
	}
	return nil
}
