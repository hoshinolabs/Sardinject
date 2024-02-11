using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class ContainerBuilder {
        Action<Container> onBuild;
        public event Action<Container> OnBuild {
            add => onBuild += value;
            remove => onBuild -= value;
        }

        List<RegistrationBuilder> registrationBuilders = new List<RegistrationBuilder>();

        public ContainerBuilder() {
        }

        public RegistrationBuilder Register(RegistrationBuilder registrationBuilder) {
            registrationBuilders.Add(registrationBuilder);
            return registrationBuilder;
        }

        public Container Build(Func<Type, Container, object> fallbackResolver = null) {
            return Build(null, fallbackResolver);
        }

        internal Container Build(Container parent, Func<Type, Container, object> fallbackResolver) {
            var registry = BuildRegistry();
            var container = new Container(parent, registry, fallbackResolver);
            onBuild(container);
            return container;
        }

        Registry BuildRegistry() {
            var registrations = BuildRegistrations();
            var registry = new Registry(registrations);
            DetectsCircularDependencies(registrations, registry);
            return registry;
        }

        Registration[] BuildRegistrations() {
            var registrations = registrationBuilders
                .Select(registrationBuilder => registrationBuilder.Build())
                .ToList();
            registrations.Insert(0, CreateContainerRegistration());
            return registrations.ToArray();
        }

        Registration CreateContainerRegistration() {
            var interfaceTypes = new HashSet<Type>(typeof(Container).GetInterfaces());
            var provider = new ContainerProvider();
            return new Registration(typeof(Container), Lifetime.Cached, interfaceTypes, provider);
        }

        void DetectsCircularDependencies(Registration[] registrations, Registry registry) {
            foreach (var registration in registrations) {
                DetectsCircularDependencies(new ReferenceInfo(registration), registry, new Stack<ReferenceInfo>());
            }
        }

        void DetectsCircularDependencies(ReferenceInfo current, Registry registry, Stack<ReferenceInfo> stack) {
            foreach (var x in stack.Select((v, i) => (v, i)).ToArray()) {
                if (current.ImplementationType == x.v.ImplementationType) {
                    stack.Push(current);
                    throw SardinjectException.CreateCircularDependency(stack.Take(x.i + 1));
                }
            }
            stack.Push(current);
            if (InjectTypeInfoCache.TryGet(current.ImplementationType, out var info)) {
                foreach (var field in info.Fields) {
                    if (registry.TryGet(field.FieldType, out var registration)) {
                        DetectsCircularDependencies(new ReferenceInfo(registration, current.Reference, field), registry, stack);
                    }
                }
                foreach (var property in info.Properties) {
                    if (registry.TryGet(property.PropertyType, out var registration)) {
                        DetectsCircularDependencies(new ReferenceInfo(registration, current.Reference, property), registry, stack);
                    }
                }
                foreach (var method in info.Methods) {
                    foreach (var parameter in method.GetParameters()) {
                        if (registry.TryGet(parameter.ParameterType, out var registration)) {
                            DetectsCircularDependencies(new ReferenceInfo(registration, current.Reference, method), registry, stack);
                        }
                    }
                }
            }
            stack.Pop();
        }
    }
}
