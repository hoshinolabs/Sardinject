using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public class BindingBuilder {
        internal event Action<Binding> OnBindingBuilt;

        internal Type ImplementationType;
        internal List<Type> InterfaceTypes = new();

        internal IResolverBuilder ResolverBuilder;

        internal object Id;

        public BindingBuilder(Type implementationType, IResolverBuilder resolverBuilder) {
            ImplementationType = implementationType;
            ResolverBuilder = resolverBuilder;
        }

        public Binding Build() {
            var resolver = ResolverBuilder.Build();
            var binding = new Binding(Id, resolver);
            OnBindingBuilt?.Invoke(binding);
            return binding;
        }
    }
}
