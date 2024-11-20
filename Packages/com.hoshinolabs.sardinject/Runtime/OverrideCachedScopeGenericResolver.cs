using System;

namespace HoshinoLabs.Sardinject {
    public sealed class OverrideCachedScopeGenericResolver : IBindingResolver {
        public readonly IGenericBindingResolver Resolver;
        public Container Container { get; private set; }

        public OverrideCachedScopeGenericResolver(IGenericBindingResolver resolver, ContainerBuilder containerBuilder) {
            Resolver = resolver;
            containerBuilder.OnContainerPreBuilt += (container) => {
                Container = container;
            };
        }

        public object Resolve(Type type, Container container) {
            var resolver = Resolver.MakeResolver(type);
            return Container.Resolve(resolver, type);
        }
    }
}
