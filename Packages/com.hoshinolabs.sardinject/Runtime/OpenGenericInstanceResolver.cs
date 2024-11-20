using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public sealed class OpenGenericInstanceResolver : IGenericBindingResolver {
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        readonly ConcurrentDictionary<Type, IBindingResolver> cache = new();

        public OpenGenericInstanceResolver(IReadOnlyDictionary<object, IResolver> parameters) {
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            return MakeResolver(type).Resolve(type, container);
        }

        public IBindingResolver MakeResolver(Type type) {
            return cache.GetOrAdd(type, (_) => {
                var injector = InjectorCache.GetOrBuild(type);
                return new InstanceResolver(injector, Parameters);
            });
        }
    }
}
