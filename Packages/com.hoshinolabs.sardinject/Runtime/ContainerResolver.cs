using System;

namespace HoshinoLabs.Sardinject {
    public sealed class ContainerResolver : IBindingResolver {
        public Container Container { get; private set; }

        public ContainerResolver(ContainerBuilder containerBuilder) {
            containerBuilder.OnContainerPreBuilt += (container) => {
                Container = container;
            };
        }

        public object Resolve(Type type, Container container) {
            return Container;
        }
    }
}
