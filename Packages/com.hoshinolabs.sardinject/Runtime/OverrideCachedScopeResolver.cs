using System;

namespace HoshinoLabs.Sardinject {
    public sealed class OverrideCachedScopeResolver : IBindingResolver {
        public readonly IBindingResolver Resolver;
        public Container Container { get; private set; }

        public OverrideCachedScopeResolver(IBindingResolver resolver, ContainerBuilder containerBuilder) {
            Resolver = resolver;
            containerBuilder.OnContainerPreBuilt += (container) => {
                Container = container;
            };
        }

        public object Resolve(Type type, Container _) {
            return Container.Resolve(Resolver, type);
        }
    }
}
