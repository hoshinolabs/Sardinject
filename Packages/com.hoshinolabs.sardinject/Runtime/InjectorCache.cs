using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class InjectorCache {
        Dictionary<Type, Injector> cache = new Dictionary<Type, Injector>();

        InjectTypeInfoCache injectTypeInfoCache = new InjectTypeInfoCache();
        public InjectTypeInfoCache InjectTypeInfoCache => injectTypeInfoCache;

        public bool TryGet(Type type, out Injector value) {
            return cache.TryGetValue(type, out value);
        }

        public Injector GetOrAdd(Type type) {
            if (cache.TryGetValue(type, out var injector)) {
                return injector;
            }
            var info = injectTypeInfoCache.GetOrAdd(type);
            injector = new Injector(info);
            cache.Add(type, injector);
            return injector;
        }

        public Injector GetOrAdd<T>() {
            return GetOrAdd(typeof(T));
        }
    }
}
