#if UDONSHARP
using UnityEditor;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal static class UdonContainerInjector {
        static GameObject rootGo;

        [InitializeOnLoadMethod]
        static void OnLoad() {
            UnityInjector.Installers -= OverrideUdonContainerInjection;
            UnityInjector.Installers += OverrideUdonContainerInjection;
        }

        static void OverrideUdonContainerInjection(ContainerBuilder builder) {
            builder.OverrideUdonContainerInjection()
                .UnderTransform(() => {
                    if (rootGo == null) {
                        rootGo = new GameObject($"__{typeof(UdonContainerInjector).Namespace.Replace('.', '_')}__");
                        rootGo.hideFlags = HideFlags.HideInHierarchy;
                    }
                    return rootGo.transform;
                });
        }
    }
}
#endif
