using System;

namespace HoshinoLabs.Sardinject {
    public sealed class ContainerBuilder {
        readonly RegistryBuilder registryBuilder = new();

        public event Action<Container> OnContainerPreBuilt;
        public event Action<Container> OnContainerBuilt;

        public ContainerBuilder() {

        }

        internal ContainerBuilder(Registry registry) {
            registryBuilder = new(registry);
        }

        public void Register<T>(T builder) where T : BindingBuilder {
            registryBuilder.Register(builder);
        }

        public Container Build() {
            var registry = registryBuilder.Build();
            OverrideContainerInjection(ref registry);
            var container = new Container(registry);
            OnContainerPreBuilt?.Invoke(container);
            OnContainerBuilt?.Invoke(container);
            return container;
        }

        void OverrideContainerInjection(ref Registry registry) {
            var resolverBuilder = new ContainerResolverBuilder(this);
            var bindingBuilder = new BindingBuilder(typeof(Container), resolverBuilder);
            var registryBuilder = new RegistryBuilder(registry);
            registryBuilder.Register(bindingBuilder);
            registry = registryBuilder.Build();
        }
    }
}
