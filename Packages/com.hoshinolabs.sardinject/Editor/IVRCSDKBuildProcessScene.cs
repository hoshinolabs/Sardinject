using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    internal abstract class IVRCSDKBuildProcessScene : IProcessSceneWithReport {
        public abstract int callbackOrder { get; }

        public void OnProcessScene(Scene scene, BuildReport report) {
            var stack = new System.Diagnostics.StackTrace(true);
            var frames = stack.GetFrames();
            if (frames.Any(x => x.GetMethod().ReflectedType == typeof(VRC.SDK3.Editor.Builder.VRCWorldBuilder))) {
                OnBuildProcessScene(scene);
            }
        }

        public abstract void OnBuildProcessScene(Scene scene);
    }
}
