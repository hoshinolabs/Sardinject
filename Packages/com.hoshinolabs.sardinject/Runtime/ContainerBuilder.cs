using System;

namespace HoshinoLabs.Sardinject {
    public class ContainerBuilder {
        readonly RegistryBuilder registryBuilder = new();

        internal event Action<Container> OnContainerBuiltInternal;
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
            OnContainerBuiltInternal?.Invoke(container);
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
