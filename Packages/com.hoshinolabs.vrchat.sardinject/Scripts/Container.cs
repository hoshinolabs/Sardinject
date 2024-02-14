using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Container {
        Container upper;
        Registry registry;
        ResolverCache cache;
        Resolver resolver;

        internal ResolverCache Cache => cache;
        public Registry Registry => registry;

        internal Container(Container upper, Registry registry, ResolverCache cache, Resolver resolver) {
            this.upper = upper;
            this.registry = registry;
            this.cache = cache;
            this.resolver = resolver;
        }

        public object Resolve(Type type) {
            if (TryGetRegistration(type, out var registration)) {
                return Resolve(registration);
            }
            if(resolver != null) {
                foreach (var x in resolver.GetInvocationList().Cast<Resolver>()) {
                    var instance = x(type, this);
                    if (instance != null) {
                        return instance;
                    }
                }
            }
            throw SardinjectException.CreateUnableResolve(type);
        }

        object Resolve(Registration registration) {
            switch (registration.Lifetime) {
                case Lifetime.Transient: {
                        // 自分で作る(毎回)
                        return registration.GetInstance(this);
                    }
                case Lifetime.Cached: {
                        // 自分に登録がなかったら親に任せる
                        if (!registry.Exists(registration.ImplementationType)) {
                            upper.Resolve(registration);
                        }
                        // 自分で作る(キャッシュ)
                        return cache.GetOrAdd(registration, this).Value;
                    }
                case Lifetime.Scoped: {
                        // 自分で作る(キャッシュ)
                        return cache.GetOrAdd(registration, this).Value;
                    }
                default: {
                        return registration.GetInstance(this);
                    }
            }
        }

        bool TryGetRegistration(Type type, out Registration registration) {
            if (registry.TryGet(type, out registration)) {
                return true;
            }
            if (upper != null) {
                return upper.TryGetRegistration(type, out registration);
            }
            return false;
        }

        public void Inject(object instance) {
            var injector = InjectorCache.GetOrAdd(instance.GetType());
            injector.Inject(instance, this, null);
        }
    }
}
