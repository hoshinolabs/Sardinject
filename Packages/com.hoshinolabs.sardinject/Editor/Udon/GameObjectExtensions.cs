using System;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    internal static class GameObjectExtensions {
        public static UdonSharpBehaviour AddUdonSharpComponent(this GameObject self, Type type, bool initialize = true) {
            if (type == typeof(UdonSharpBehaviour)) {
                throw new ArgumentException("Cannot add components of type 'UdonSharpBehaviour', you can only add subclasses of this type");
            }

            if (!typeof(UdonSharpBehaviour).IsAssignableFrom(type)) {
                throw new ArgumentException("Type for AddUdonSharpComponent must be a subclass of UdonSharpBehaviour");
            }

            var proxyBehaviour = (UdonSharpBehaviour)self.AddComponent(type);
#if !VRC_CLIENT && UNITY_EDITOR
            var runBehaviourSetup = typeof(UdonSharpEditor.UdonSharpEditorUtility)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.Name == "RunBehaviourSetup")
                .Where(x => x.GetParameters().Length == 1)
                .First();
            runBehaviourSetup.Invoke(null, new[] { proxyBehaviour });
#endif

#if !VRC_CLIENT && UNITY_EDITOR
            if (initialize && UnityEditor.EditorApplication.isPlaying) {
                UdonSharpEditor.UdonSharpEditorUtility.GetBackingUdonBehaviour(proxyBehaviour).InitializeUdonContent();
            }
#endif

            return proxyBehaviour;
        }

        public static T AddUdonSharpComponent<T>(this GameObject self, bool initialize = true) where T : UdonSharpBehaviour {
            return (T)AddUdonSharpComponent(self, typeof(T), initialize);
        }
    }
}
