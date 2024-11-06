using System;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class EntryPointsBuilder {
        readonly ContainerBuilder containerBuilder;

        public EntryPointsBuilder(ContainerBuilder containerBuilder) {
            this.containerBuilder = containerBuilder;
        }

        public ComponentBindingBuilder Register<T>(Lifetime lifetime) where T : Component {
            return containerBuilder.RegisterComponent<T>(lifetime)
                .EnsureBindingResolved(containerBuilder);
        }

        public ComponentBindingBuilder RegisterComponent<T>(T component) {
            return containerBuilder.RegisterComponentInstance(component);
        }

        public ComponentBindingBuilder RegisterInHierarchy(Type type) {
            return containerBuilder.RegisterComponentInHierarchy(type);
        }

        public ComponentBindingBuilder RegisterInHierarchy<T>() where T : Component {
            return containerBuilder.RegisterComponentInHierarchy<T>();
        }

        public ComponentBindingBuilder RegisterInNewPrefab(Type type, Lifetime lifetime, GameObject prefab) {
            return containerBuilder.RegisterComponentInNewPrefab(type, lifetime, prefab)
                .EnsureBindingResolved(containerBuilder);
        }

        public ComponentBindingBuilder RegisterInNewPrefab<T>(Lifetime lifetime, GameObject prefab) where T : Component {
            return containerBuilder.RegisterComponentInNewPrefab<T>(lifetime, prefab)
                .EnsureBindingResolved(containerBuilder);
        }

        public ComponentBindingBuilder RegisterOnNewGameObject(Type type, Lifetime lifetime, string gameObjectName = null) {
            return containerBuilder.RegisterComponentOnNewGameObject(type, lifetime, gameObjectName)
                .EnsureBindingResolved(containerBuilder);
        }

        public ComponentBindingBuilder RegisterOnNewGameObject<T>(Lifetime lifetime, string gameObjectName = null) where T : Component {
            return containerBuilder.RegisterComponentOnNewGameObject<T>(lifetime, gameObjectName)
                .EnsureBindingResolved(containerBuilder);
        }
    }
}
