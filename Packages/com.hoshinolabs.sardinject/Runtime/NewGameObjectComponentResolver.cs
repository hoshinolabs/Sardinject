using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class NewGameObjectComponentResolver : IBindingResolver {
        public readonly Type ComponentType;
        public readonly string GameObjectName;
        public readonly ComponentDestination Destination;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public NewGameObjectComponentResolver(Type componentType, string gameObjectName, ComponentDestination destination, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            ComponentType = componentType;
            GameObjectName = gameObjectName;
            Destination = destination;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            var gameObjectName = string.IsNullOrEmpty(GameObjectName)
                ? ComponentType.Name
                : GameObjectName;
            var go = new GameObject(gameObjectName);
            go.SetActive(false);
            var transform = Destination.Transform?.Resolve<Transform>(container);
            if (transform != null) {
                go.transform.SetParent(transform);
            }
#if UDONSHARP
            var component = typeof(UdonSharp.UdonSharpBehaviour).IsAssignableFrom(ComponentType)
                ? go.AddUdonSharpComponent(ComponentType, false)
                : go.AddComponent(ComponentType);
#else
            var component = go.AddComponent(ComponentType);
#endif
            Injector.Inject(component, container, Parameters);
            Destination.ApplyDontDestroyOnLoadIfNeeded(component);
            go.SetActive(true);
            return component;
        }
    }
}
