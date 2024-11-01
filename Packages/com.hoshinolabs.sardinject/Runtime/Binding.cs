namespace HoshinoLabs.Sardinject {
    public class Binding {
        public readonly object Id;
        public readonly IResolver Resolver;

        public Binding(object id, IResolver resolver) {
            Id = id;
            Resolver = resolver;
        }
    }
}
