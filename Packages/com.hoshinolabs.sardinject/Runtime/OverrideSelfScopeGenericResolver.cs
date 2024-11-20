using System;

namespace HoshinoLabs.Sardinject {
    public sealed class OverrideSelfScopeGenericResolver : IBindingResolver {
        public readonly IGenericBindingResolver Resolver;

        public OverrideSelfScopeGenericResolver(IGenericBindingResolver resolver) {
            Resolver = resolver;
        }

        public object Resolve(Type type, Container container) {
            var resolver = Resolver.MakeResolver(type);
            return container.Resolve(resolver, type);
        }
    }
}
