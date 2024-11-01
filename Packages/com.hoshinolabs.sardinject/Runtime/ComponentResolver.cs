using System;
using System.Collections.Generic;
#if UDONSHARP
using UdonSharp;
#endif
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class ComponentResolver : IResolver {
        public readonly Type ComponentType;
        ComponentDestination Destination;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public ComponentResolver(Type componentType, ComponentDestination destination, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            ComponentType = componentType;
            Destination = destination;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Container container) {
            var transform = Destination.Transform?.Resolve<Transform>(container);
            var go = transform?.gameObject
                ?? new GameObject(ComponentType.Name);
            go.SetActive(false);
#if UDONSHARP
            var component = typeof(UdonSharpBehaviour).IsAssignableFrom(ComponentType)
                ? go.AddUdonSharpComponentEx(ComponentType, false)
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
