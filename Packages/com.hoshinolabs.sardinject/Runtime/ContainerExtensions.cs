namespace HoshinoLabs.Sardinject {
    public static class ContainerExtensions {
        public static T Resolve<T>(this Container self) {
            return (T)self.ResolveOrId(typeof(T), null);
        }
    }
}
