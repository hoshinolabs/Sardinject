using System;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class ComponentBindingBuilder : BindingBuilder {
        ComponentDestination destination;

        public ComponentBindingBuilder(Type implementationType, IResolverBuilder resolverBuilder, ComponentDestination destination)
            : base(implementationType, resolverBuilder) {
            this.destination = destination;
        }

        public ComponentBindingBuilder UnderTransform(Transform transform) {
            destination.Transform = new ParameterResolver(transform);
            return this;
        }

        public ComponentBindingBuilder UnderTransform(Func<Transform> transform) {
            destination.Transform = new ParameterFactoryResolver(_ => transform());
            return this;
        }

        public ComponentBindingBuilder UnderTransform(Func<Container, Transform> transform) {
            destination.Transform = new ParameterFactoryResolver(container => transform(container));
            return this;
        }

        public ComponentBindingBuilder DontDestroyOnLoad() {
            destination.DontDestroyOnLoad = true;
            return this;
        }
    }
}
