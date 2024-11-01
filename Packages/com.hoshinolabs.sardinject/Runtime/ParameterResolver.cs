namespace HoshinoLabs.Sardinject {
    public sealed class ParameterResolver : IResolver {
        public readonly object Value;

        public ParameterResolver(object value) {
            Value = value;
        }

        public object Resolve(Container container) {
            return Value;
        }
    }
}
