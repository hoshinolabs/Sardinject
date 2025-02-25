using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public sealed class FactoryResolver : IBindingResolver {
        public readonly IResolver Resolver;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public FactoryResolver(IResolver resolver, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            Resolver = resolver;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            var instance = Resolver.Resolve(container);
            Injector.Inject(instance, container, Parameters);
            return instance;
        }
    }
}
