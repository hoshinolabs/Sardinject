using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class ExistenceComponentResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly object component;
        readonly ComponentDestination destination;

        public ExistenceComponentResolverBuilder(Type type, object component, ComponentDestination destination) {
            this.type = type;
            this.component = component;
            this.destination = destination;
        }

        public IBindingResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new ExistenceComponentResolver(component, destination, injector, Parameters);
        }
    }
}
