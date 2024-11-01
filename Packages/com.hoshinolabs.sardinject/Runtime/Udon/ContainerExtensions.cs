#if UDONSHARP
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;

namespace HoshinoLabs.Sardinject.Udon {
    public static class ContainerExtensions {
        public static T Resolve<T>(this IContainer self, bool _ = default) {
            return (T)((Container)self).ResolveOrId(typeof(T), null);
        }

        public static T Resolve<T>(this IContainer self) where T : UdonSharpBehaviour {
            return (T)((Container)self).ResolveOrId(UdonSharpBehaviour.GetUdonTypeName<T>(), null);
        }
    }
}
#endif
