using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public class FindComponentResolver : IResolver {
        public readonly Type ComponentType;
        public readonly ComponentDestination Destination;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public FindComponentResolver(Type componentType, ComponentDestination destination, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            ComponentType = componentType;
            Destination = destination;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Container container) {
            var transform = Destination.Transform?.Resolve<Transform>(container);
            var component = transform?.GetComponentInChildren(ComponentType, true)
                ?? FindComponentInScene();
            if (component == null) {
                throw new SardinjectException($"{ComponentType} type component not found{(transform ? $" in {transform.name} children" : "")}.");
            }
            Injector.Inject(component, container, Parameters);
            Destination.ApplyDontDestroyOnLoadIfNeeded(component);
            return component;
        }

        Component FindComponentInScene() {
            foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects()) {
                var component = go.GetComponentInChildren(ComponentType, true);
                if (component == null) {
                    continue;
                }
                return component;
            }
            return null;
        }
    }
}
