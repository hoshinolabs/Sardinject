using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class ContainerResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        readonly ContainerBuilder containerBuilder;

        public ContainerResolverBuilder(ContainerBuilder containerBuilder) {
            this.containerBuilder = containerBuilder;
        }

        public IBindingResolver Build() {
            return new ContainerResolver(containerBuilder);
        }
    }
}
