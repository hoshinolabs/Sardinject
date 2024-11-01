using System;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ComponentContainerBuilderExtensions {
        public static void UseComponents(this ContainerBuilder self, Action<ComponentsBuilder> configuration) {
            configuration(new ComponentsBuilder(self));
        }

        public static void UseComponents(this ContainerBuilder self, Transform transform, Action<ComponentsBuilder> configuration) {
            configuration(new ComponentsBuilder(self, transform));
        }

        public static ComponentBindingBuilder RegisterComponent<T>(this ContainerBuilder self, Lifetime lifetime) where T : Component {
            var destination = new ComponentDestination();
            var resolverBuilder = new ComponentResolverBuilder(typeof(T), destination).OverrideScopeIfNeeded(self, lifetime);
            var builder = new ComponentBindingBuilder(typeof(T), resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentInstance<T>(this ContainerBuilder self, T component) {
            var destination = new ComponentDestination();
            var resolverBuilder = new ExistenceComponentResolverBuilder(component.GetType(), component, destination).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(component.GetType(), resolverBuilder, destination);
            var resolver = new Lazy<IResolver>();
            builder.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            builder.As<T>();
            self.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(container);
            };
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentInHierarchy(this ContainerBuilder self, Type type) {
            var destination = new ComponentDestination();
            var resolverBuilder = new FindComponentResolverBuilder(type, destination).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(type, resolverBuilder, destination);
            var resolver = new Lazy<IResolver>();
            builder.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            self.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(container);
            };
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentInHierarchy<T>(this ContainerBuilder self) where T : Component {
            var destination = new ComponentDestination();
            var resolverBuilder = new FindComponentResolverBuilder(typeof(T), destination).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(typeof(T), resolverBuilder, destination);
            var resolver = new Lazy<IResolver>();
            builder.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            self.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(container);
            };
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentInNewPrefab(this ContainerBuilder self, Type type, Lifetime lifetime, GameObject prefab) {
            var destination = new ComponentDestination();
            var resolverBuilder = new NewPrefabComponentResolverBuilder(type, prefab, destination).OverrideScopeIfNeeded(self, lifetime);
            var builder = new ComponentBindingBuilder(type, resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentInNewPrefab<T>(this ContainerBuilder self, Lifetime lifetime, GameObject prefab) where T : Component {
            var destination = new ComponentDestination();
            var resolverBuilder = new NewPrefabComponentResolverBuilder(typeof(T), prefab, destination).OverrideScopeIfNeeded(self, lifetime);
            var builder = new ComponentBindingBuilder(typeof(T), resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentOnNewGameObject(this ContainerBuilder self, Type type, Lifetime lifetime, string gameObjectName = null) {
            var destination = new ComponentDestination();
            var resolverBuilder = new NewGameObjectComponentResolverBuilder(type, gameObjectName, destination).OverrideScopeIfNeeded(self, lifetime);
            var builder = new ComponentBindingBuilder(type, resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }

        public static ComponentBindingBuilder RegisterComponentOnNewGameObject<T>(this ContainerBuilder self, Lifetime lifetime, string gameObjectName = null) where T : Component {
            var destination = new ComponentDestination();
            var resolverBuilder = new NewGameObjectComponentResolverBuilder(typeof(T), gameObjectName, destination).OverrideScopeIfNeeded(self, lifetime);
            var builder = new ComponentBindingBuilder(typeof(T), resolverBuilder, destination);
            self.Register(builder);
            return builder;
        }
    }
}
