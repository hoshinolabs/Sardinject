#if UDONSHARP
using System.Reflection;
using UdonSharp;

namespace HoshinoLabs.Sardinject {
    internal static class UdonSharpBehaviourExtensions {
        public static void SetPublicVariable<T>(this UdonSharpBehaviour self, string symbolName, T value) {
            var field = self.GetType().GetField(symbolName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(self, value);
        }
    }
}
#endif
