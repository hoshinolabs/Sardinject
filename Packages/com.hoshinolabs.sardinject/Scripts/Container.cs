using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Container {
        Container container;
        Registry registry;
        Resolver resolver;
        ResolverCache resolverCache;

        public Registry Registry => registry;

        internal Container(Container container, Registry registry, Resolver resolver, ResolverCache resolverCache) {
            this.container = container;
            this.registry = registry;
            this.resolver = resolver;
            this.resolverCache = resolverCache;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type) {
            return Resolve(type, new Attribute[0]);
        }

        public object Resolve(Type type, Attribute[] attributes) {
            if (TryGetRegistration(type, out var registration)) {
                return Resolve(registration);
            }
            return ResolveFallback(type, attributes);
        }

        object ResolveFallback(Type type, Attribute[] attributes) {
            if (resolver != null) {
                foreach (var x in resolver.GetInvocationList().Cast<Resolver>()) {
                    var instance = x(this, type, attributes);
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
                        return registration.GetInstance(this);
                    }
                case Lifetime.Cached: {
                        if (!registry.Exists(registration.ImplementationType)) {
                            return container.Resolve(registration);
                        }
                        return resolverCache.GetOrAdd(registration, this).Value;
                    }
                case Lifetime.Scoped: {
                        return resolverCache.GetOrAdd(registration, this).Value;
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
            if (container != null) {
                return container.TryGetRegistration(type, out registration);
            }
            return false;
        }

        public void Inject(object instance) {
            var injector = InjectorCache.GetOrAdd(instance.GetType());
            injector.Inject(instance, this, null, null);
        }
    }
}
