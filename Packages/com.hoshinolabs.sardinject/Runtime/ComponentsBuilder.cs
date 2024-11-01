using System;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class ComponentsBuilder {
        readonly ContainerBuilder containerBuilder;
        readonly Transform transform;

        public ComponentsBuilder(ContainerBuilder containerBuilder, Transform transform = null) {
            this.containerBuilder = containerBuilder;
            this.transform = transform;
        }

        public ComponentBindingBuilder Register<T>(T component) {
            return containerBuilder.RegisterComponentInstance(component);
        }

        public ComponentBindingBuilder RegisterInHierarchy(Type type) {
            return containerBuilder.RegisterComponentInHierarchy(type)
                .UnderTransform(transform);
        }

        public ComponentBindingBuilder RegisterInHierarchy<T>() where T : Component {
            return containerBuilder.RegisterComponentInHierarchy<T>()
                .UnderTransform(transform);
        }

        public ComponentBindingBuilder RegisterInNewPrefab(Type type, Lifetime lifetime, GameObject prefab) {
            return containerBuilder.RegisterComponentInNewPrefab(type, lifetime, prefab)
                .UnderTransform(transform);
        }

        public ComponentBindingBuilder RegisterInNewPrefab<T>(Lifetime lifetime, GameObject prefab) where T : Component {
            return containerBuilder.RegisterComponentInNewPrefab<T>(lifetime, prefab)
                .UnderTransform(transform);
        }

        public ComponentBindingBuilder RegisterOnNewGameObject(Type type, Lifetime lifetime, string gameObjectName = null) {
            return containerBuilder.RegisterComponentOnNewGameObject(type, lifetime, gameObjectName)
                .UnderTransform(transform);
        }

        public ComponentBindingBuilder RegisterOnNewGameObject<T>(Lifetime lifetime, string gameObjectName = null) where T : Component {
            return containerBuilder.RegisterComponentOnNewGameObject<T>(lifetime, gameObjectName)
                .UnderTransform(transform);
        }
    }
}
