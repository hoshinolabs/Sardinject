using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Context {
        Context context;

        Resolver resolver = default;
        public event Resolver Resolver {
            add => resolver += value;
            remove => resolver -= value;
        }

        RegistrationCache registrationCache = new RegistrationCache();
        ResolverCache resolverCache = new ResolverCache();
        InjectorCache injectorCache = new InjectorCache();

        Container container;
        public Container Container => container;

        List<Installer> installers = new List<Installer>();

        public Context() {

        }

        public Context New() {
            var context = new Context();
            context.context = this;
            context.resolver = resolver;
            return context;
        }

        public Container Build() {
            context?.Build();
            var builder = new ContainerBuilder(context?.container, resolver, registrationCache, resolverCache, injectorCache);
            builder.OnBuild += container => {
                this.container = container;
            };
            InstallTo(builder);
            return builder.Build();
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
