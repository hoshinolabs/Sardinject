using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class SardinjectPreBuilder : IProcessSceneWithReport {
        public int callbackOrder => -5000;

        public void OnProcessScene(Scene scene, BuildReport report) {
            ProjectContextHelper.Instance = new Context(SardinjectBuilder.FallbackResolver);
        }
    }
}
