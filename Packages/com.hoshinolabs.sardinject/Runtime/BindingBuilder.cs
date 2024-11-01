using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public class BindingBuilder {
        internal event Action<Binding> OnBindingBuilt;

        internal Type ImplementationType;
        internal List<Type> InterfaceTypes = new();

        readonly IResolverBuilder resolverBuilder;

        object id;

        public BindingBuilder(Type implementationType, IResolverBuilder resolverBuilder) {
            ImplementationType = implementationType;
            this.resolverBuilder = resolverBuilder;
        }

        public Binding Build() {
            var resolver = resolverBuilder.Build();
            var binding = new Binding(id, resolver);
            OnBindingBuilt?.Invoke(binding);
            return binding;
        }

        public BindingBuilder As(params Type[] interfaceTypes) {
            foreach (var interfaceType in interfaceTypes) {
                AddInterfaceType(interfaceType);
            }
            return this;
        }

        public BindingBuilder As<T>() {
            AddInterfaceType(typeof(T));
            return this;
        }

        public BindingBuilder As<T1, T2>() {
            AddInterfaceType(typeof(T1));
            AddInterfaceType(typeof(T2));
            return this;
        }

        public BindingBuilder As<T1, T2, T3>() {
            AddInterfaceType(typeof(T1));
            AddInterfaceType(typeof(T2));
            AddInterfaceType(typeof(T3));
            return this;
        }

        public BindingBuilder As<T1, T2, T3, T4>() {
            AddInterfaceType(typeof(T1));
            AddInterfaceType(typeof(T2));
            AddInterfaceType(typeof(T3));
            AddInterfaceType(typeof(T4));
            return this;
        }

        void AddInterfaceType(Type interfaceType) {
            if (!interfaceType.IsAssignableFrom(ImplementationType)) {
                throw new SardinjectException($"`{ImplementationType}` is not assingnable from `{interfaceType}`.");
            }
            InterfaceTypes.Add(interfaceType);
        }

        public BindingBuilder AsImplementedInterfaces() {
            foreach (var interfaceType in ImplementationType.GetInterfaces()) {
                AddInterfaceType(interfaceType);
            }
            return this;
        }

        public BindingBuilder AsSelf() {
            AddInterfaceType(ImplementationType);
            return this;
        }

        public BindingBuilder WithParameter(string name, object value) {
            resolverBuilder.Parameters.Add(name, new ParameterResolver(value));
            return this;
        }

        public BindingBuilder WithParameter(string name, Func<object> value) {
            resolverBuilder.Parameters.Add(name, new ParameterFactoryResolver(_ => value()));
            return this;
        }

        public BindingBuilder WithParameter(string name, Func<Container, object> value) {
            resolverBuilder.Parameters.Add(name, new ParameterFactoryResolver(container => value(container)));
            return this;
        }

        public BindingBuilder WithParameter(Type type, object value) {
            resolverBuilder.Parameters.Add(type, new ParameterResolver(value));
            return this;
        }

        public BindingBuilder WithParameter(Type type, Func<object> value) {
            resolverBuilder.Parameters.Add(type, new ParameterFactoryResolver(_ => value()));
            return this;
        }

        public BindingBuilder WithParameter(Type type, Func<Container, object> value) {
            resolverBuilder.Parameters.Add(type, new ParameterFactoryResolver(container => value(container)));
            return this;
        }

        public BindingBuilder WithParameter<T>(T value) {
            resolverBuilder.Parameters.Add(typeof(T), new ParameterResolver(value));
            return this;
        }

        public BindingBuilder WithParameter<T>(Func<T> value) {
            resolverBuilder.Parameters.Add(typeof(T), new ParameterFactoryResolver(_ => value()));
            return this;
        }

        public BindingBuilder WithParameter<T>(Func<Container, T> value) {
            resolverBuilder.Parameters.Add(typeof(T), new ParameterFactoryResolver(container => value(container)));
            return this;
        }

        public BindingBuilder WithId(object id) {
            this.id = id;
            return this;
        }
    }
}
