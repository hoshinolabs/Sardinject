using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class NewGameObjectComponentResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly string gameObjectName;
        readonly ComponentDestination destination;

        public NewGameObjectComponentResolverBuilder(Type type, string gameObjectName, ComponentDestination destination) {
            this.type = type;
            this.gameObjectName = gameObjectName;
            this.destination = destination;
        }

        public IBindingResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new NewGameObjectComponentResolver(type, gameObjectName, destination, injector, Parameters);
        }
    }
}
