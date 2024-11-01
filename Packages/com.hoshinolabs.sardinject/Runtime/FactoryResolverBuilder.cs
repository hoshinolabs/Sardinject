using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class FactoryResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;
        readonly IResolver resolver;

        public FactoryResolverBuilder(Type type, IResolver resolver) {
            this.type = type;
            this.resolver = resolver;
        }

        public IResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new FactoryResolver(resolver, injector, Parameters);
        }
    }
}
