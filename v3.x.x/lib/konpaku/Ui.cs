using System;
using System.Collections.Generic;
using UnityEngine;

namespace Konpaku
{
    internal class Ui : MonoBehaviour
    {
        internal static List<string> Functions;

        private static Rect _windowRect;

        internal static Ui Inst { get; set; }

        internal static void Initialize()
        {
            if (Functions == null)
            {
                Functions = new List<string>();
                foreach (var mod in Enum.GetValues(typeof(Mods)))
                    if ((Mods)mod != 0) Functions.Add(mod.ToString().Replace("_", "+"));
            }

            _windowRect.min = new Vector2(20f, 20f);

            if (Inst == null)
                Inst = new GameObject("Konpaku").AddComponent<Ui>();

            DontDestroyOnLoad(Inst);
        }

        private static void MainWindow(int id)
        {
            UiTemplate.Initialize(Progress.CurrentState);
        }

        private void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1024f, Screen.height / 576f, 1f));
            if (Inst != null)
                _windowRect = GUILayout.Window(0, _windowRect, MainWindow, GUIContent.none, GUIStyle.none);
        }
    }
}