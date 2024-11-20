using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class OverrideCachedScopeResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters => resolverBuilder.Parameters;

        readonly IResolverBuilder resolverBuilder;
        readonly ContainerBuilder containerBuilder;

        public OverrideCachedScopeResolverBuilder(IResolverBuilder resolverBuilder, ContainerBuilder containerBuilder) {
            this.resolverBuilder = resolverBuilder;
            this.containerBuilder = containerBuilder;
        }

        public IBindingResolver Build() {
            return new OverrideCachedScopeResolver(resolverBuilder.Build(), containerBuilder);
        }
    }
}
