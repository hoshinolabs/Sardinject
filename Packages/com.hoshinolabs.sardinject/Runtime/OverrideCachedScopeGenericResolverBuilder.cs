using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class OverrideCachedScopeGenericResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters => resolverBuilder.Parameters;

        readonly IGenericResolverBuilder resolverBuilder;
        readonly ContainerBuilder containerBuilder;

        public OverrideCachedScopeGenericResolverBuilder(IGenericResolverBuilder resolverBuilder, ContainerBuilder containerBuilder) {
            this.resolverBuilder = resolverBuilder;
            this.containerBuilder = containerBuilder;
        }

        public IBindingResolver Build() {
            var resolver = (IGenericBindingResolver)resolverBuilder.Build();
            return new OverrideCachedScopeGenericResolver(resolver, containerBuilder);
        }
    }
}
