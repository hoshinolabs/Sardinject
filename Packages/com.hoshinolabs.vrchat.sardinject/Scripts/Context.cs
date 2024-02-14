using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Context {
        Context upper;
        Resolver resolver;

        RegistrationCache registrationCache = new RegistrationCache();
        ResolverCache resolverCache = new ResolverCache();

        Container container;

        public Container Container => container;

        List<Installer> installers = new List<Installer>();

        public Context(Resolver resolver = null) {
            this.resolver = resolver;
        }

        public Context New(Resolver resolver = null) {
            var context = new Context();
            context.upper = this;
            context.resolver = this.resolver + resolver;
            return context;
        }

        public void Build() {
            upper?.Build();
            var builder = new ContainerBuilder(upper?.container, resolver, registrationCache, resolverCache);
            builder.OnBuild += container => {
                this.container = container;
            };
            InstallTo(builder);
            builder.Build();
        }

        public void Enqueue(Action<IContainerBuilder> configuration) {
            installers.Add(new Installer(configuration));
        }

        void InstallTo(IContainerBuilder builder) {
            foreach (var installer in installers) {
                installer.Install(builder);
            }
            installers.Clear();
        }
    }
}
