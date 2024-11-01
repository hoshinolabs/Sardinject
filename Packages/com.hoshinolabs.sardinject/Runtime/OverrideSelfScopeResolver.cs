namespace HoshinoLabs.Sardinject {
    public sealed class OverrideSelfScopeResolver : IResolver {
        public readonly IResolver Resolver;

        public OverrideSelfScopeResolver(IResolver resolver) {
            Resolver = resolver;
        }

        public object Resolve(Container container) {
            return container.Resolve(Resolver);
        }
    }
}
