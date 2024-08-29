using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharp;
#if UNITY_EDITOR
using UdonSharpEditor;
using UnityEditor;
#endif
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal static class UdonSharpComponentExtensions {
        internal static UdonSharpBehaviour AddUdonSharpComponentEx(this GameObject self, Type type, bool initialize) {
            if (type == typeof(UdonSharpBehaviour)) {
                throw new ArgumentException("Cannot add components of type 'UdonSharpBehaviour', you can only add subclasses of this type");
            }

            if (!typeof(UdonSharpBehaviour).IsAssignableFrom(type)) {
                throw new ArgumentException("Type for AddUdonSharpComponent must be a subclass of UdonSharpBehaviour");
            }

            var proxyBehaviour = (UdonSharpBehaviour)self.AddComponent(type);
#if UNITY_EDITOR
            var runBehaviourSetup = typeof(UdonSharpEditorUtility).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.Name == "RunBehaviourSetup")
                .Where(x => x.GetParameters().Length == 1)
                .First();
            runBehaviourSetup.Invoke(null, new[] { proxyBehaviour });
#endif

#if UNITY_EDITOR
            if (initialize && EditorApplication.isPlaying) {
                UdonSharpEditorUtility.GetBackingUdonBehaviour(proxyBehaviour).InitializeUdonContent();
            }
#endif

            return proxyBehaviour;
        }

        internal static T AddUdonSharpComponentEx<T>(this GameObject self, bool initialize) where T : UdonSharpBehaviour {
            return (T)AddUdonSharpComponentEx(self, typeof(T), initialize);
        }
    }
}
