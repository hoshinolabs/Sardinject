using HoshinoLabs.Sardinject.Udon;
using UnityEditor;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal static class UdonSharpInjector {
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
                        rootGo = new GameObject($"__{typeof(IContainer).Namespace.Replace('.', '_')}__");
                        rootGo.hideFlags = HideFlags.HideInHierarchy;
                    }
                    return rootGo.transform;
                });
        }
    }
}
