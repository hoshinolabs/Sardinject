using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public static class SceneInjector {
        public static event Action<Scene> OnSceneLoaded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad() {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;

#if UNITY_EDITOR
            if (IsDisableScenereload) {
                var scenes = Enumerable.Range(0, UnityEditor.SceneManagement.EditorSceneManager.sceneCount)
                    .Select(x => UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(x));
                foreach (var scene in scenes) {
                    SceneLoaded(scene);
                }
            }
#endif
        }

#if UNITY_EDITOR
        static bool IsDisableScenereload {
            get {
                var enterPlayModeOptionsEnabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled;
                var enterPlayModeOptions = UnityEditor.EditorSettings.enterPlayModeOptions;
                return enterPlayModeOptionsEnabled
                    && enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableSceneReload);
            }
        }
#endif

        static void SceneLoaded(Scene scene, LoadSceneMode _) {
            SceneLoaded(scene);
        }

        static void SceneLoaded(Scene scene) {
            OnSceneLoaded?.Invoke(scene);
        }
    }
}
