using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    internal sealed class SardinjectBuilder : IVRCSDKBuildProcessScene {
        public override int callbackOrder => 1;

        public override void OnBuildProcessScene(Scene scene) {
            var onSubsystemRegistrationMethod = typeof(UnityInjector).GetMethod("OnSubsystemRegistration", BindingFlags.Static | BindingFlags.NonPublic);
            onSubsystemRegistrationMethod.Invoke(null, Array.Empty<object>());
            var onBeforeSceneLoadMethod = typeof(SceneInjector).GetMethod("OnBeforeSceneLoad", BindingFlags.Static | BindingFlags.NonPublic);
            onBeforeSceneLoadMethod.Invoke(null, Array.Empty<object>());
            var sceneLoadedMethod = typeof(SceneInjector).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.Name == "SceneLoaded")
                .Where(x => x.GetParameters().Length == 1)
                .First();
            sceneLoadedMethod.Invoke(null, new object[] { scene });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad() {
            if (!IsDisableSceneReload) {
                return;
            }

            var scenes = Enumerable.Range(0, EditorSceneManager.sceneCount)
                .Select(x => EditorSceneManager.GetSceneAt(x));
            foreach (var scene in scenes) {
                var sceneLoadedMethod = typeof(SceneInjector).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(x => x.Name == "SceneLoaded")
                    .Where(x => x.GetParameters().Length == 1)
                    .First();
                sceneLoadedMethod.Invoke(null, new object[] { scene });
            }
        }

        static bool IsDisableSceneReload {
            get {
                var enterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
                var enterPlayModeOptions = EditorSettings.enterPlayModeOptions;
                return enterPlayModeOptionsEnabled
                    && enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableSceneReload);
            }
        }
    }
}
