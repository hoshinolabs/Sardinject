using System;

namespace HoshinoLabs.Sardinject {
    internal static class ComponentBindingBuilderExtensions {
        public static ComponentBindingBuilder EnsureBindingResolved(this ComponentBindingBuilder self, ContainerBuilder builder) {
            var resolver = new Lazy<IResolver>();
            self.OnBindingBuilt += (binding) => {
                resolver = new(binding.Resolver);
            };
            builder.OnContainerBuilt += (container) => {
                resolver.Value.Resolve(container);
            };
            return self;
        }
    }
}
