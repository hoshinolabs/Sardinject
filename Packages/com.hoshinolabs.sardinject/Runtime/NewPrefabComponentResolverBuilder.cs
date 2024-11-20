using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class NewPrefabComponentResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly GameObject prefab;
        readonly ComponentDestination destination;

        public NewPrefabComponentResolverBuilder(Type type, GameObject prefab, ComponentDestination destination) {
            this.type = type;
            this.prefab = prefab;
            this.destination = destination;
        }

        public IBindingResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new NewPrefabComponentResolver(type, prefab, destination, injector, Parameters);
        }
    }
}
