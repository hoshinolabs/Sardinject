using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class FindComponentResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly ComponentDestination destination;

        public FindComponentResolverBuilder(Type type, ComponentDestination destination) {
            this.type = type;
            this.destination = destination;
        }

        public IBindingResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new FindComponentResolver(type, destination, injector, Parameters);
        }
    }
}
