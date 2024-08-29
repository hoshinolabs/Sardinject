using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class RegistrationBuilder {
        Type implementationType;
        Lifetime lifetime;
        InstanceProviderBase provider;
        HashSet<Type> interfaceTypes = new HashSet<Type>();

        public Type ImplementationType => implementationType;
        public HashSet<Type> InterfaceTypes => interfaceTypes;

        internal RegistrationBuilder(Type implementationType, Lifetime lifetime, InstanceProviderBase provider) {
            this.implementationType = implementationType;
            this.lifetime = lifetime;
            this.provider = provider;
        }

        internal Registration Build(InjectorCache injectorCache) {
            var injector = injectorCache.GetOrAdd(implementationType);
            provider.Build(injector, implementationType);
            return new Registration(ImplementationType, lifetime, InterfaceTypes, provider);
        }

        public RegistrationBuilder As<T>() {
            return As(typeof(T));
        }

        public RegistrationBuilder As(Type interfaceType) {
            AddInterfaceType(interfaceType);
            return this;
        }

        public RegistrationBuilder As(params Type[] interfaceTypes) {
            foreach (var interfaceType in interfaceTypes) {
                AddInterfaceType(interfaceType);
            }
            return this;
        }

        void AddInterfaceType(Type interfaceType) {
            if (!interfaceType.IsAssignableFrom(implementationType)) {
                throw SardinjectException.CreateNotAssignableFrom(implementationType, interfaceType);
            }
            interfaceTypes.Add(interfaceType);
        }

        public RegistrationBuilder AsImplementedInterfaces() {
            foreach (var interfaceType in implementationType.GetInterfaces()
                .Where(x => !typeof(UdonSharpBehaviour).IsAssignableFrom(x))) {
                interfaceTypes.Add(interfaceType);
            }
            return this;
        }

        public RegistrationBuilder AsSelf() {
            AddInterfaceType(implementationType);
            return this;
        }

        public RegistrationBuilder WithId(object id) {
            provider.Id = id;
            return this;
        }

        public RegistrationBuilder WithParameter<T>(T value) {
            return WithParameter(typeof(T), value);
        }

        public RegistrationBuilder WithParameter(Type type, object value) {
            provider.Parameters.Add(type, value);
            return this;
        }

        public RegistrationBuilder WithParameter(string name, object value) {
            provider.Parameters.Add(name, value);
            return this;
        }

        public RegistrationBuilder UnderTransform(Transform parent) {
            provider.GetTransform = () => parent;
            return this;
        }

        public RegistrationBuilder UnderTransform(Func<Transform> func) {
            provider.GetTransform = func;
            return this;
        }
    }
}
