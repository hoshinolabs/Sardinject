using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public static class InjectorCache {
        static Dictionary<Type, Injector> cache;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            cache = new Dictionary<Type, Injector>();
        }

        public static bool TryGet(Type type, out Injector value) {
            return cache.TryGetValue(type, out value);
        }

        public static Injector GetOrAdd(Type type) {
            if (cache.TryGetValue(type, out var injector)) {
                return injector;
            }
            var info = InjectTypeInfoCache.GetOrAdd(type);
            injector = new Injector(info);
            cache.Add(type, injector);
            return injector;
        }

        public static Injector GetOrAdd<T>() {
            return GetOrAdd(typeof(T));
        }
    }
}
