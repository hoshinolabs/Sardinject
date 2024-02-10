using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Container {
        Container parent;
        Registry registry;
        Func<Type, Container, object> fallbackResolver;

        Dictionary<Registration, Lazy<object>> cache = new Dictionary<Registration, Lazy<object>>();

        public Registry Registry => registry;

        internal Container(Container parent, Registry registry, Func<Type, Container, object> fallbackResolver) {
            this.parent = parent;
            this.registry = registry;
            this.fallbackResolver = fallbackResolver;
        }

        public object Resolve(Type type) {
            return Resolve(type, this);
        }

        object Resolve(Type type, Container scope) {
            if (registry.TryGet(type, out var registration)) {
                return Resolve(registration, scope);
            }
            return FallbackResolve(type, scope);
        }

        object Resolve(Registration registration, Container scope) {
            switch (registration.Lifetime) {
                case Lifetime.Transient: {
                        return registration.GetInstance(this);
                    }
                case Lifetime.Cached: {
                        if (!cache.TryGetValue(registration, out var value)) {
                            value = new(() => registration.GetInstance(this));
                            cache.Add(registration, value);
                        }
                        return value.Value;
                    }
                case Lifetime.Scoped: {
                        if (!scope.cache.TryGetValue(registration, out var value)) {
                            value = new(() => registration.GetInstance(this));
                            scope.cache.Add(registration, value);
                        }
                        return value.Value;
                    }
                default: {
                        return registration.GetInstance(this);
                    }
            }
        }

        object FallbackResolve(Type type, Container scope) {
            if (parent != null) {
                return parent.Resolve(type, scope);
            }
            if (fallbackResolver != null) {
                var value = fallbackResolver(type, scope);
                if (value != null) {
                    return value;
                }
            }
            throw SardinjectException.CreateUnableResolve(type);
        }

        public void Inject(object instance) {
            throw new NotImplementedException();
        }
    }
}
