#if UDONSHARP
using System;
using System.Reflection;
using UdonSharp;
using VRC.Udon;

namespace HoshinoLabs.Sardinject {
    internal static class UdonSharpBehaviourExtensions {
        public static void ApplyProxyModifications(this UdonSharpBehaviour self) {
#if UNITY_EDITOR
            var udon = UdonSharpEditor.UdonSharpEditorUtility.GetBackingUdonBehaviour(self);
            var ClearBehaviourVariablesMethod = typeof(UdonSharpEditor.UdonSharpEditorUtility).GetMethod("ClearBehaviourVariables", BindingFlags.Static | BindingFlags.NonPublic);
            ClearBehaviourVariablesMethod.Invoke(null, new object[] { udon, true });
            var preBuildSerializeField = typeof(UdonSharpEditor.ProxySerializationPolicy).GetField("PreBuildSerialize", BindingFlags.Static | BindingFlags.NonPublic);
            var serializationPolicy = (UdonSharpEditor.ProxySerializationPolicy)preBuildSerializeField.GetValue(null);
            UdonSharpEditor.UdonSharpEditorUtility.CopyProxyToUdon(self, serializationPolicy);
            var serializePublicVariablesMethod = typeof(UdonBehaviour).GetMethod("SerializePublicVariables", BindingFlags.Instance | BindingFlags.NonPublic);
            serializePublicVariablesMethod.Invoke(udon, Array.Empty<object>());
#endif
        }
    }
}
#endif
