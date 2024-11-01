using System;

namespace HoshinoLabs.Sardinject {
    public sealed class Installer : IInstaller {
        Action<ContainerBuilder> configuration;

        public Installer(Action<ContainerBuilder> configuration) {
            this.configuration = configuration;
        }

        public void Install(ContainerBuilder builder) {
            configuration(builder);
        }
    }
}
