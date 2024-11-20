using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class OverrideSelfScopeGenericResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters => resolverBuilder.Parameters;

        readonly IGenericResolverBuilder resolverBuilder;

        public OverrideSelfScopeGenericResolverBuilder(IGenericResolverBuilder resolverBuilder) {
            this.resolverBuilder = resolverBuilder;
        }

        public IBindingResolver Build() {
            var resolver = (IGenericBindingResolver)resolverBuilder.Build();
            return new OverrideSelfScopeGenericResolver(resolver);
        }
    }
}
