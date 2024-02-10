using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UdonSharp;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal static class UdonSharpBehaviourExtensions {
        internal static void SetPublicVariable<T>(this UdonSharpBehaviour self, string symbolName, T value) {
            var field = self.GetType().GetField(symbolName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(self, value);
        }
    }
}
