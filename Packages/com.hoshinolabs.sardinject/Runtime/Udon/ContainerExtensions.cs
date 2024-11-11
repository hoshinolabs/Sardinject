using UdonSharp;

namespace HoshinoLabs.Sardinject.Udon {
    public static class ContainerExtensions {
        public static T Resolve<T>(this Container self, bool _ = default) {
            return (T)((ContainerShim)self).ResolveOrId(typeof(T), null);
        }

        public static T Resolve<T>(this Container self) where T : UdonSharpBehaviour {
            return (T)((ContainerShim)self).ResolveOrId(UdonSharpBehaviour.GetUdonTypeName<T>(), null);
        }
    }
}
