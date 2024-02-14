using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Data;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class SardinjectBuilder : IProcessSceneWithReport {
        public int callbackOrder => 1;

        public void OnProcessScene(Scene scene, BuildReport report) {
            try {
                ProjectContextHelper.Build();
            }
            catch (SardinjectException e) {
                EditorApplication.isPlaying = false;
                Debug.LogError($"[<color=#47F1FF>Sardinject</color>] {e.Message}");

                throw e;
            }
        }
    }
}
