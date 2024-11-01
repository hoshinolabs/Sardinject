using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class OverrideSelfScopeResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters => resolverBuilder.Parameters;

        readonly IResolverBuilder resolverBuilder;

        public OverrideSelfScopeResolverBuilder(IResolverBuilder resolverBuilder) {
            this.resolverBuilder = resolverBuilder;
        }

        public IResolver Build() {
            return new OverrideSelfScopeResolver(resolverBuilder.Build());
        }
    }
}
