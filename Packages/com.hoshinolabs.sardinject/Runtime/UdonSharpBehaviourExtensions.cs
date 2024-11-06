#if UDONSHARP
using System;
using System.Reflection;
using UdonSharp;
#if UNITY_EDITOR
using UdonSharpEditor;
#endif
using VRC.Udon;

namespace HoshinoLabs.Sardinject {
    internal static class UdonSharpBehaviourExtensions {
        public static void SetPublicVariable<T>(this UdonSharpBehaviour self, string symbolName, T value) {
            var field = self.GetType().GetField(symbolName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(self, value);
        }

        public static void ApplyProxyModifications(this UdonSharpBehaviour self) {
#if UNITY_EDITOR
            var udon = UdonSharpEditorUtility.GetBackingUdonBehaviour(self);
            var ClearBehaviourVariablesMethod = typeof(UdonSharpEditorUtility).GetMethod("ClearBehaviourVariables", BindingFlags.Static | BindingFlags.NonPublic);
            ClearBehaviourVariablesMethod.Invoke(null, new object[] { udon, true });
            var preBuildSerializeField = typeof(ProxySerializationPolicy).GetField("PreBuildSerialize", BindingFlags.Static | BindingFlags.NonPublic);
            var serializationPolicy = (ProxySerializationPolicy)preBuildSerializeField.GetValue(null);
            UdonSharpEditorUtility.CopyProxyToUdon(self, serializationPolicy);
            var serializePublicVariablesMethod = typeof(UdonBehaviour).GetMethod("SerializePublicVariables", BindingFlags.Instance | BindingFlags.NonPublic);
            serializePublicVariablesMethod.Invoke(udon, Array.Empty<object>());
#endif
        }
    }
}
#endif
