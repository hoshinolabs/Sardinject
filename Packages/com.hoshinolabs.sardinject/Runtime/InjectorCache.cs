using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal static class InjectorCache {
        static readonly Dictionary<Type, Injector> cache = new();

        public static Injector GetOrBuild(Type type) {
            if (!cache.TryGetValue(type, out var injector)) {
                var info = InjectTypeInfoCache.GetOrBuild(type);
                injector = new Injector(info);
                cache.Add(type, injector);
            }
            return injector;
        }
    }
}
