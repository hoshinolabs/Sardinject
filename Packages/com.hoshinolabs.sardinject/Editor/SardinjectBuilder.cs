using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    internal sealed class SardinjectBuilder : IVRCSDKBuildProcessScene {
        public override int callbackOrder => 1;

        public override void OnBuildProcessScene(Scene scene) {
            var onSubsystemRegistrationMethod = typeof(UnityInjector).GetMethod("OnSubsystemRegistration", BindingFlags.Static | BindingFlags.NonPublic);
            onSubsystemRegistrationMethod.Invoke(null, Array.Empty<object>());
            var sceneLoadedMethod = typeof(SceneInjector).GetMethod("SceneLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            sceneLoadedMethod.Invoke(null, new object[] { scene });
        }
    }
}
