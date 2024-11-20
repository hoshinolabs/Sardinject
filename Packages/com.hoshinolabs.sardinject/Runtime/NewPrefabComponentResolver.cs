using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class NewPrefabComponentResolver : IBindingResolver {
        public readonly Type ComponentType;
        public readonly GameObject Prefab;
        public readonly ComponentDestination Destination;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public NewPrefabComponentResolver(Type componentType, GameObject prefab, ComponentDestination destination, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            ComponentType = componentType;
            Prefab = prefab;
            Destination = destination;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            var active = Prefab.activeSelf;
            if (active) {
                Prefab.SetActive(false);
            }
            var transform = Destination.Transform?.Resolve<Transform>(container);
            var instance = transform
                ? GameObject.Instantiate(Prefab, transform)
                : GameObject.Instantiate(Prefab);
            instance.name = Prefab.name;
            var component = instance.GetComponentInChildren(ComponentType);
            if (component == null) {
                throw new SardinjectException($"{ComponentType} type component not found{(transform ? $" in {transform.name} children" : "")}.");
            }
            Injector.Inject(component, container, Parameters);
            Destination.ApplyDontDestroyOnLoadIfNeeded(component);
            if (active) {
                Prefab.SetActive(active);
                instance.SetActive(active);
            }
            return component;
        }
    }
}
