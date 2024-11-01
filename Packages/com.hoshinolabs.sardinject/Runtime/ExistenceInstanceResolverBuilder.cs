using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class ExistenceInstanceResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly object instance;

        public ExistenceInstanceResolverBuilder(Type type, object instance) {
            this.type = type;
            this.instance = instance;
        }

        public IResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new ExistenceInstanceResolver(instance, injector, Parameters);
        }
    }
}
