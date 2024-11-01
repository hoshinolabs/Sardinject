using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class InstanceResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly Type type;

        public InstanceResolverBuilder(Type type) {
            this.type = type;
        }

        public IResolver Build() {
            var injector = InjectorCache.GetOrBuild(type);
            return new InstanceResolver(injector, Parameters);
        }
    }
}
