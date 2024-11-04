using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class ResolverCache {
        readonly Dictionary<IResolver, Lazy<object>> cache = new();

        public object GetOrAdd(IResolver resolver, Func<object> factory) {
            if (!cache.TryGetValue(resolver, out var value)) {
                value = new(() => factory.Invoke());
                cache.Add(resolver, value);
            }
            return value.Value;
        }
    }
}