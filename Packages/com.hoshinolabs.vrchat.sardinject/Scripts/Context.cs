using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Context : IContext {
        Context parent;

        Container container;
        public Container Container => container;

        List<Installer> installers = new List<Installer>();

        public void Build(Func<Type, Container, object> fallbackResolver = null) {
            parent?.Build(fallbackResolver);

            var builder = new ContainerBuilder();
            builder.OnBuild += container => {
                this.container = container;
            };
            InstallTo(builder);
            builder.Build(parent?.Container, fallbackResolver);
        }

        public Context CreateChild() {
            var context = new Context();
            context.parent = this;
            return context;
        }

        public void Push(Action<ContainerBuilder> configuration) {
            installers.Add(new Installer(configuration));
        }

        void InstallTo(ContainerBuilder builder) {
            foreach (var installer in installers) {
                installer.Install(builder);
            }
            installers.Clear();
        }
    }
}
