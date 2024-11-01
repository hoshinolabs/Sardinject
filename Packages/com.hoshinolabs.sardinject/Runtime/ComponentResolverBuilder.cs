using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class ComponentResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly ComponentDestination destination;

        public ComponentResolverBuilder(Type type, ComponentDestination destination) {
            this.type = type;
            this.destination = destination;
        }

        public IResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new ComponentResolver(type, destination, injector, Parameters);
        }
    }
}
