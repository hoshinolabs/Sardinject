using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Container {
        readonly ResolverCache cache = new();

        public readonly Registry Registry;

        internal Container(Registry registry) {
            Registry = registry;
        }

        public Container Scope(Action<ContainerBuilder> configuration = null) {
            var builder = new ContainerBuilder(Registry);
            configuration?.Invoke(builder);
            return builder.Build();
        }

        public object Resolve(Type type) {
            return ResolveOrId(type, null);
        }

        internal object ResolveOrId(Type type, object id) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                var elementType = type.GenericTypeArguments.First();
                return Registry.GetBindings(elementType)
                    .Where(x => id == null || id.Equals(x.Id))
                    .Select(x => Resolve(x))
                    .Cast(elementType);
            }
            if (type.IsArray) {
                var elementType = type.GetElementType();
                return Registry.GetBindings(elementType)
                    .Where(x => id == null || id.Equals(x.Id))
                    .Select(x => Resolve(x))
                    .Cast(elementType)
                    .ToArray(elementType);
            }
            var binding = Registry.GetBindings(type)
                .Reverse()
                .Where(x => id == null || id.Equals(x.Id))
                .FirstOrDefault();
            if (binding == null) {
                throw new SardinjectException($"Unable to resolve for type `{type.FullName}`.");
            }
            return Resolve(binding);
        }

        internal object Resolve(Binding binding) {
            return binding.Resolver.Resolve(this);
        }

        internal object Resolve(IResolver resolver) {
            return cache.GetOrAdd(resolver, () => resolver.Resolve(this));
        }

        internal object ResolveOrParameterOrId(string name, Type type, object id, IReadOnlyDictionary<object, IResolver> parameters) {
            if (parameters.TryGetValue(name, out var nameParameter)) {
                return nameParameter.Resolve(this);
            }
            if (parameters.TryGetValue(type, out var typeParameter)) {
                return typeParameter.Resolve(this);
            }
            return ResolveOrId(type, id);
        }

        public void Inject(object instance) {
            var injector = InjectorCache.GetOrBuild(instance.GetType());
            injector.Inject(instance, this, new Dictionary<object, IResolver>());
        }
    }
}
