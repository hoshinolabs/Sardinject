namespace HoshinoLabs.Sardinject {
    public static class ResolverExtensions {
        public static T Resolve<T>(this IResolver self, Container container) {
            return (T)self.Resolve(container);
        }
    }
}
