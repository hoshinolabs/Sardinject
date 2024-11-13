namespace HoshinoLabs.Sardinject.Udon {
    internal static class ComponentContainerBuilderExtensions {
        public static ComponentBindingBuilder OverrideContainerInjection(this ContainerBuilder self) {
            var destination = new ComponentDestination();
            var resolverBuilder = new ContainerResolverBuilder(destination).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(typeof(Container), resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }
    }
}
