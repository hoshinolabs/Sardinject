using System;

namespace HoshinoLabs.Sardinject {
    public static class ContainerBuilderExtensions {
        public static BindingBuilder Register(this ContainerBuilder self, Type type, Lifetime lifetime) {
            var resolverBuilder = type.IsGenericTypeDefinition
                ? new OpenGenericInstanceResolverBuilder().OverrideScopeIfNeeded(self, lifetime)
                : new InstanceResolverBuilder(type).OverrideScopeIfNeeded(self, lifetime);
            var builder = new BindingBuilder(type, resolverBuilder);
            self.Register(builder);
            return builder;
        }

        public static BindingBuilder Register<T>(this ContainerBuilder self, Lifetime lifetime) {
            var resolverBuilder = typeof(T).IsGenericTypeDefinition
                ? new OpenGenericInstanceResolverBuilder().OverrideScopeIfNeeded(self, lifetime)
                : new InstanceResolverBuilder(typeof(T)).OverrideScopeIfNeeded(self, lifetime);
            var builder = new BindingBuilder(typeof(T), resolverBuilder);
            self.Register(builder);
            return builder;
        }

        public static BindingBuilder RegisterInstance<T>(this ContainerBuilder self, T instance) {
            var resolverBuilder = new ExistenceInstanceResolverBuilder(instance.GetType(), instance).OverrideScopeIfNeeded(self, Lifetime.Cached);
            var builder = new BindingBuilder(instance.GetType(), resolverBuilder).As<T>();
            self.Register(builder);
            return builder;
        }

        public static BindingBuilder RegisterFactory<T>(this ContainerBuilder self, Func<T> factory, Lifetime lifetime) {
            var resolverBuilder = new FactoryResolverBuilder(typeof(T), new ParameterFactoryResolver(_ => factory())).OverrideScopeIfNeeded(self, lifetime);
            var builder = new BindingBuilder(typeof(T), resolverBuilder);
            self.Register(builder);
            return builder;
        }

        public static BindingBuilder RegisterFactory<T>(this ContainerBuilder self, Func<Container, T> factory, Lifetime lifetime) {
            var resolverBuilder = new FactoryResolverBuilder(typeof(T), new ParameterFactoryResolver(container => factory(container))).OverrideScopeIfNeeded(self, lifetime);
            var builder = new BindingBuilder(typeof(T), resolverBuilder);
            self.Register(builder);
            return builder;
        }
    }
}
