using System.Collections.Generic;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class ContainerResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = null;

        readonly ComponentDestination destination;

        public ContainerResolverBuilder(ComponentDestination destination) {
            this.destination = destination;
        }

        public IResolver Build() {
            return new ContainerResolver(destination);
        }
    }
}
