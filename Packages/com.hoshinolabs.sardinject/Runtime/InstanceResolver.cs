using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public sealed class InstanceResolver : IBindingResolver {
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public InstanceResolver(Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            return Injector.Construct(container, Parameters);
        }
    }
}
