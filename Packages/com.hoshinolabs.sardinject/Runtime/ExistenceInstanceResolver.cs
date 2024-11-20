using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public sealed class ExistenceInstanceResolver : IBindingResolver {
        public readonly object Instance;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public ExistenceInstanceResolver(object instance, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            Instance = instance;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            Injector.Inject(Instance, container, Parameters);
            return Instance;
        }
    }
}
