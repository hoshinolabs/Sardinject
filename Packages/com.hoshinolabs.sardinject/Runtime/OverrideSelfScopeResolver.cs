using System;

namespace HoshinoLabs.Sardinject {
    public sealed class OverrideSelfScopeResolver : IBindingResolver {
        public readonly IBindingResolver Resolver;

        public OverrideSelfScopeResolver(IBindingResolver resolver) {
            Resolver = resolver;
        }

        public object Resolve(Type type, Container container) {
            return container.Resolve(Resolver, type);
        }
    }
}
