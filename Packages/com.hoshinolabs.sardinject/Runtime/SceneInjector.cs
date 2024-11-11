using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public static class SceneInjector {
        public static event Action<Scene> OnSceneLoaded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad() {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        static void SceneLoaded(Scene scene, LoadSceneMode _) {
            SceneLoaded(scene);
        }

        static void SceneLoaded(Scene scene) {
            OnSceneLoaded?.Invoke(scene);
        }
    }
}
