using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class ExistenceComponentResolver : IBindingResolver {
        public readonly object Instance;
        public readonly ComponentDestination Destination;
        public readonly Injector Injector;
        public readonly IReadOnlyDictionary<object, IResolver> Parameters;

        public ExistenceComponentResolver(object instance, ComponentDestination destination, Injector injector, IReadOnlyDictionary<object, IResolver> parameters) {
            Instance = instance;
            Destination = destination;
            Injector = injector;
            Parameters = parameters;
        }

        public object Resolve(Type type, Container container) {
            if (!typeof(Component).IsAssignableFrom(Instance.GetType())) {
                throw new SardinjectException($"{Instance.GetType()} is not a Component.");
            }
            var component = (Component)Instance;
            Injector.Inject(component, container, Parameters);
            Destination.ApplyDontDestroyOnLoadIfNeeded(component);
            return component;
        }
    }
}
