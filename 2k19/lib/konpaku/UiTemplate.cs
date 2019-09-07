using System;
using System.IO;
using UnityEngine;

namespace Konpaku
{
    public class UiTemplate
    {
        internal static void Initialize(Progress.States currentState)
        {
            if (currentState == Progress.States.BeginInitializingPackage)
                BeginInitializingPackage();

            if (currentState == Progress.States.BeginSelectingMod)
                BeginSelectingMod();
        }

        private static void BeginInitializingPackage()
        {
            GUILayout.BeginVertical(UiStyle.Style[0]);
            GUILayout.Box("Initializing Package...", UiStyle.Style[1]);
            GUILayout.EndVertical();
        }

        private static void BeginSelectingMod()
        {
            var isInitialized = false;
            GUILayout.BeginVertical(UiStyle.Style[0]);
            for (var i = 0; i < Ui.Functions.Count; i++)
            {
                if (i == 0) GUILayout.BeginHorizontal();
                if (i == 0 || i == 3) GUILayout.BeginVertical();

                if (!isInitialized)
                {
                    if (GUILayout.Button("I'm not a cheater!", UiStyle.Style[5]))
                    {
                        Main.SelectedMod = 0;
                        Progress.CurrentState = Progress.States.EndSelectingMod;
                    }
                    isInitialized = true;
                }

                var isToggleAble = Directory.Exists(PathMgr.Raw(((Mods[])Enum.GetValues(typeof(Mods)))[i + 1].ToString().ToLower().Replace("_", "-")));
                if (GUILayout.Button(Ui.Functions[i], UiStyle.Style[isToggleAble ? 5 : 6]))
                {
                    if (!isToggleAble)
                        break;

                    Main.SelectedMod = ((Mods[])Enum.GetValues(typeof(Mods)))[i + 1];
                    Progress.CurrentState = Progress.States.EndSelectingMod;
                }

                if (i == 2 || i == 6) GUILayout.EndVertical();
                if (i == 6) GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal(UiStyle.Style[0]);
            if (GUILayout.Button("Replace Skin", UiStyle.Style[Main.ReplaceSkin ? 7 : 8]))
            {
                Main.ReplaceSkin = !Main.ReplaceSkin;
                File.WriteAllText(PathMgr.WorkingDirectory("prefs"), Main.ReplaceSkin ? "1" : "0");
            }
            GUILayout.EndHorizontal();
        }
    }
}