using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Registration {
        Type implementationType;
        Lifetime lifetime;
        HashSet<Type> interfaceTypes;

        InstanceProviderBase provider;

        public Type ImplementationType => implementationType;
        public Lifetime Lifetime => lifetime;
        public HashSet<Type> InterfaceTypes => interfaceTypes;

        public bool IsRaw => provider.IsRaw;
        public bool IsPrefab => provider.IsPrefab;

        internal Registration(Type implementationType, Lifetime lifetime, HashSet<Type> interfaceTypes, InstanceProviderBase provider) {
            this.implementationType = implementationType;
            this.lifetime = lifetime;
            this.interfaceTypes = interfaceTypes;
            this.provider = provider;
        }

        public object GetInstance(Container container) {
            return provider.GetInstance(container);
        }
    }
}
