using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Konpaku
{
    internal enum Mods
    {
        None,
        GodMode,
        WeakEnemy,
        GodMode_Damage,
        GodMode_Cooldown,
        GodMode_WeakEnemy,
        GodMode_Damage_Cooldown,
        GodMode_Damage_WeakEnemy,
        GodMode_Damage_Cooldown_WeakEnemy
    }

    public class Main
    {
        internal static bool ReplaceSkin;
        internal static Mods SelectedMod;

        public static IEnumerator Initialize(Action callback, string version)
        {
            if (!Directory.Exists(PathMgr.WorkingDirectory()))
                Directory.CreateDirectory(PathMgr.WorkingDirectory());

            Ui.Initialize();
            yield return InitializePackage("raw", version);

            if (!File.Exists(PathMgr.WorkingDirectory("prefs")))
                File.WriteAllText(PathMgr.WorkingDirectory("prefs"), "0");

            ReplaceSkin = File.ReadAllText(PathMgr.WorkingDirectory("prefs")) != "0";

            Progress.CurrentState = Progress.States.BeginSelectingMod;
            yield return new WaitUntil(() => Progress.CurrentState == Progress.States.EndSelectingMod);

            callback();
        }

        private static IEnumerator InitializePackage(string fileName, string version)
        {
            Progress.CurrentState = Progress.States.BeginInitializingPackage;

            var localPackagePath = PathMgr.WorkingDirectory(fileName + ".zip");
            var webPackagePath = PathMgr.Web(version, LocalizationVersion(), fileName + ".zip");
            var isCached = false;

            yield return HttpClient.GetHeader(webPackagePath, "Content-Length", (isErr, err, length) =>
            {
                if (!isErr && File.Exists(localPackagePath) && new FileInfo(localPackagePath).Length.ToString() == length)
                    isCached = true;
            });

            if (!isCached)
            {
                yield return HttpClient.GetBytes(webPackagePath, (isErr, err, bytes) =>
                {
                    if (!isErr)
                        File.WriteAllBytes(localPackagePath, bytes);
                });
            }

            if (Directory.Exists(PathMgr.WorkingDirectory(fileName)))
                Rmdir(PathMgr.WorkingDirectory(fileName));

            var fastZip = new FastZip();
            fastZip.ExtractZip(localPackagePath, PathMgr.WorkingDirectory(), null);
            Progress.CurrentState = Progress.States.EndInitializingPackage;
        }

        private static string LocalizationVersion() => Application.identifier == "com.YoStarJP.AzurLane" ? "jp" : Application.identifier == "com.YoStarEN.AzurLane" ? "en" : Application.identifier == "kr.txwy.and.blhx" ? "kr" : "cn";

        private static void Rmdir(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
                Rmdir(directory);

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}