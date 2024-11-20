using System;

namespace HoshinoLabs.Sardinject {
    public interface IBindingResolver {
        object Resolve(Type type, Container container);
    }

    public interface IGenericBindingResolver : IBindingResolver {
        IBindingResolver MakeResolver(Type type);
    }

    public sealed class Binding {
        public readonly object Id;
        public readonly IBindingResolver Resolver;

        public Binding(object id, IBindingResolver resolver) {
            Id = id;
            Resolver = resolver;
        }

        public object Resolve(Type type, Container container) {
            return Resolver.Resolve(type, container);
        }
    }
}
