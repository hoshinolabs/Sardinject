#if UDONSHARP
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonContainerResolverBuilder : IResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = null;

        readonly ComponentDestination destination;

        public UdonContainerResolverBuilder(ComponentDestination destination) {
            this.destination = destination;
        }

        public IResolver Build() {
            return new UdonContainerResolver(destination);
        }
    }
}
#endif
