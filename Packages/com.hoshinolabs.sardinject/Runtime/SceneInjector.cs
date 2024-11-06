using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public static class SceneInjector {
        public static event Action<Scene> OnSceneLoaded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad() {
            void _SceneLoaded(Scene scene, LoadSceneMode _) {
                SceneLoaded(scene);
            };

            SceneManager.sceneLoaded -= _SceneLoaded;
            SceneManager.sceneLoaded += _SceneLoaded;

#if UNITY_EDITOR
            var enterPlayModeOptionsEnabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled;
            var enterPlayModeOptions = UnityEditor.EditorSettings.enterPlayModeOptions;
            var manualSceneLoad = enterPlayModeOptionsEnabled
                && enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableSceneReload);
            if (manualSceneLoad) {
                var scenes = Enumerable.Range(0, UnityEditor.SceneManagement.EditorSceneManager.sceneCount)
                    .Select(x => UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(x));
                foreach (var scene in scenes) {
                    SceneLoaded(scene);
                }
            }
#endif
        }

        static void SceneLoaded(Scene scene) {
            OnSceneLoaded.Invoke(scene);
        }
    }
}
