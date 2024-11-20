using System;

namespace HoshinoLabs.Sardinject {
    public static class ComponentBindingBuilderExtensions {
        public static ComponentBindingBuilder As(this ComponentBindingBuilder self, params Type[] interfaceTypes) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.As(self, interfaceTypes);
        }

        public static ComponentBindingBuilder As<T>(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.As<T>(self);
        }

        public static ComponentBindingBuilder As<T1, T2>(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.As<T1, T2>(self);
        }

        public static ComponentBindingBuilder As<T1, T2, T3>(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.As<T1, T2, T3>(self);
        }

        public static ComponentBindingBuilder As<T1, T2, T3, T4>(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.As<T1, T2, T3, T4>(self);
        }

        public static ComponentBindingBuilder AsImplementedInterfaces(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.AsImplementedInterfaces(self);
        }

        public static ComponentBindingBuilder AsSelf(this ComponentBindingBuilder self) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.AsSelf(self);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, string name, object value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, name, value);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, string name, Func<object> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, name, value);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, string name, Func<Container, object> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, name, value);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, Type type, object value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, type, value);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, Type type, Func<object> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, type, value);
        }

        public static ComponentBindingBuilder WithParameter(this ComponentBindingBuilder self, Type type, Func<Container, object> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, type, value);
        }

        public static ComponentBindingBuilder WithParameter<T>(this ComponentBindingBuilder self, T value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, value);
        }

        public static ComponentBindingBuilder WithParameter<T>(this ComponentBindingBuilder self, Func<T> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, value);
        }

        public static ComponentBindingBuilder WithParameter<T>(this ComponentBindingBuilder self, Func<Container, T> value) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithParameter(self, value);
        }

        public static ComponentBindingBuilder WithId(this ComponentBindingBuilder self, object id) {
            return (ComponentBindingBuilder)BindingBuilderExtensions.WithId(self, id);
        }

        internal static ComponentBindingBuilder EnsureBindingResolved(this ComponentBindingBuilder self, Type type, ContainerBuilder builder) {
            var resolver = new Lazy<IBindingResolver>();
            self.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            builder.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(type, container);
            };
            return self;
        }

        internal static ComponentBindingBuilder EnsureBindingResolved<T>(this ComponentBindingBuilder self, ContainerBuilder builder) {
            var resolver = new Lazy<IBindingResolver>();
            self.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            builder.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(typeof(T), container);
            };
            return self;
        }
    }
}
