namespace HoshinoLabs.Sardinject {
    public sealed class ContainerResolver : IResolver {
        Container container;

        public ContainerResolver(ContainerBuilder containerBuilder) {
            containerBuilder.OnContainerBuiltInternal += (container) => {
                this.container = container;
            };
        }

        public object Resolve(Container _) {
            return container;
        }
    }
}
