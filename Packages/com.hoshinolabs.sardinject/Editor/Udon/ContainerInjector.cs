using UnityEditor;
using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    internal static class ContainerInjector {
        static GameObject rootGo;

        [InitializeOnLoadMethod]
        static void OnLoad() {
            UnityInjector.Installers -= OverrideContainerInjection;
            UnityInjector.Installers += OverrideContainerInjection;
        }

        static void OverrideContainerInjection(ContainerBuilder builder) {
            builder.OverrideContainerInjection()
                .UnderTransform(() => {
                    if (rootGo == null) {
                        rootGo = new GameObject($"__{typeof(ContainerInjector).Namespace.Replace('.', '_')}__");
                        rootGo.hideFlags = HideFlags.HideInHierarchy;
                    }
                    return rootGo.transform;
                });
        }
    }
}
