using System;

namespace HoshinoLabs.Sardinject {
    public sealed class ParameterFactoryResolver : IResolver {
        public readonly Func<Container, object> Func;

        public ParameterFactoryResolver(Func<Container, object> func) {
            Func = func;
        }

        public object Resolve(Container container) {
            return Func.Invoke(container);
        }
    }
}
