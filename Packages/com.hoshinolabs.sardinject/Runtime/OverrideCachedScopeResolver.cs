namespace HoshinoLabs.Sardinject {
    public sealed class OverrideCachedScopeResolver : IResolver {
        public readonly IResolver Resolver;
        public Container Container { get; private set; }

        public OverrideCachedScopeResolver(IResolver resolver, ContainerBuilder containerBuilder) {
            Resolver = resolver;
            containerBuilder.OnContainerPreBuilt += (container) => {
                Container = container;
            };
        }

        public object Resolve(Container _) {
            return Container.Resolve(Resolver);
        }
    }
}
