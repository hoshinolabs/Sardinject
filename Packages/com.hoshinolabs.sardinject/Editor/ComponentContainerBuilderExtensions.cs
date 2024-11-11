#if UDONSHARP
namespace HoshinoLabs.Sardinject {
    internal static class ComponentContainerBuilderExtensions {
        public static ComponentBindingBuilder OverrideUdonContainerInjection(this ContainerBuilder self) {
            var destination = new ComponentDestination();
            var resolverBuilder = new UdonContainerResolverBuilder(destination).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(typeof(Udon.Container), resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }
    }
}
#endif
