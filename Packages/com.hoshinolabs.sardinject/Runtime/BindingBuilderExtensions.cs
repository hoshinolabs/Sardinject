using System;

namespace HoshinoLabs.Sardinject {
    public static class BindingBuilderExtensions {
        public static BindingBuilder As(this BindingBuilder self, params Type[] interfaceTypes) {
            foreach (var interfaceType in interfaceTypes) {
                if (!interfaceType.IsAssignableFrom(self.ImplementationType)) {
                    throw new SardinjectException($"`{self.ImplementationType}` is not assingnable from `{interfaceType}`.");
                }
                self.InterfaceTypes.Add(interfaceType);
            }
            return self;
        }

        public static BindingBuilder As<T>(this BindingBuilder self) {
            return self.As(typeof(T));
        }

        public static BindingBuilder As<T1, T2>(this BindingBuilder self) {
            return self.As(typeof(T1), typeof(T2));
        }

        public static BindingBuilder As<T1, T2, T3>(this BindingBuilder self) {
            return self.As(typeof(T1), typeof(T2), typeof(T3));
        }

        public static BindingBuilder As<T1, T2, T3, T4>(this BindingBuilder self) {
            return self.As(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public static BindingBuilder AsImplementedInterfaces(this BindingBuilder self) {
            return self.As(self.ImplementationType.GetInterfaces());
        }

        public static BindingBuilder AsSelf(this BindingBuilder self) {
            return self.As(self.ImplementationType);
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, string name, object value) {
            self.ResolverBuilder.Parameters.Add(name, new ParameterResolver(value));
            return self;
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, string name, Func<object> value) {
            self.ResolverBuilder.Parameters.Add(name, new ParameterFactoryResolver(_ => value()));
            return self;
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, string name, Func<Container, object> value) {
            self.ResolverBuilder.Parameters.Add(name, new ParameterFactoryResolver(container => value(container)));
            return self;
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, Type type, object value) {
            self.ResolverBuilder.Parameters.Add(type, new ParameterResolver(value));
            return self;
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, Type type, Func<object> value) {
            self.ResolverBuilder.Parameters.Add(type, new ParameterFactoryResolver(_ => value()));
            return self;
        }

        public static BindingBuilder WithParameter(this BindingBuilder self, Type type, Func<Container, object> value) {
            self.ResolverBuilder.Parameters.Add(type, new ParameterFactoryResolver(container => value(container)));
            return self;
        }

        public static BindingBuilder WithParameter<T>(this BindingBuilder self, T value) {
            self.ResolverBuilder.Parameters.Add(typeof(T), new ParameterResolver(value));
            return self;
        }

        public static BindingBuilder WithParameter<T>(this BindingBuilder self, Func<T> value) {
            self.ResolverBuilder.Parameters.Add(typeof(T), new ParameterFactoryResolver(_ => value()));
            return self;
        }

        public static BindingBuilder WithParameter<T>(this BindingBuilder self, Func<Container, T> value) {
            self.ResolverBuilder.Parameters.Add(typeof(T), new ParameterFactoryResolver(container => value(container)));
            return self;
        }

        public static BindingBuilder WithId(this BindingBuilder self, object id) {
            self.Id = id;
            return self;
        }
    }
}
