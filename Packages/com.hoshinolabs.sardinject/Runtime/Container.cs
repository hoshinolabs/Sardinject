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
        InjectorCache injectorCache;

        public Registry Registry => registry;

        internal Container(Container container, Registry registry, Resolver resolver, ResolverCache resolverCache, InjectorCache injectorCache) {
            this.container = container;
            this.registry = registry;
            this.resolver = resolver;
            this.resolverCache = resolverCache;
            this.injectorCache = injectorCache;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type) {
            if (type.IsArray) {
                type = type.GetElementType();
                var objs = default(List<object>);
                if (TryGetRegistrations(type, out var registrations)) {
                    if (objs == null) {
                        objs = new List<object>();
                    }
                    objs.AddRange(registrations.Select(x => (object[])Resolve(x)).SelectMany(x => x));
                }
                if (resolver != null) {
                    foreach (var x in resolver.GetInvocationList().Cast<Resolver>()) {
                        var instance = x(this, type);
                        if (instance != null) {
                            if (objs == null) {
                                objs = new List<object>();
                            }
                            objs.Add(instance);
                        }
                    }
                }
                if (objs != null) {
                    objs = objs.Distinct().ToList();
                    var array = Array.CreateInstance(type, objs.Count);
                    Array.Copy(objs.ToArray(), array, objs.Count);
                    return array;
                }
                throw SardinjectException.CreateUnableResolve(type);
            }
            if (TryGetRegistration(type, out var registration)) {
                var objs = (object[])Resolve(registration);
                return 0 < objs.Length ? objs.First() : null;
            }
            return ResolveFallback(type);
        }

        object ResolveFallback(Type type) {
            if (resolver != null) {
                foreach (var x in resolver.GetInvocationList().Cast<Resolver>()) {
                    var instance = x(this, type);
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

        bool TryGetRegistrations(Type type, out IEnumerable<Registration> registrations) {
            var _registrations = new HashSet<Registration>();
            if (registry.TryGet(type, out var _registrations1)) {
                foreach (var registration in _registrations1) {
                    _registrations.Add(registration);
                }
            }
            if (container != null && container.TryGetRegistrations(type, out var _registrations2)) {
                foreach (var registration in _registrations2) {
                    _registrations.Add(registration);
                }
            }
            registrations = 0 < _registrations.Count ? _registrations : null;
            return 0 < _registrations.Count;
        }

        bool TryGetRegistration(Type type, out Registration registration) {
            registration = null;
            if (TryGetRegistrations(type, out var registrations)) {
                registration = registrations.First();
                return true;
            }
            return false;
        }

        public void Inject(object instance) {
            var injector = injectorCache.GetOrAdd(instance.GetType());
            injector.Inject(instance, this, null, null);
        }
    }
}
