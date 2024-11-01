using System;
using System.Collections.Generic;
using System.Linq;

namespace HoshinoLabs.Sardinject {
    internal class RegistryBuilder {
        readonly Registry registry = new();
        readonly List<BindingBuilder> builders = new();

        public RegistryBuilder() {

        }

        public RegistryBuilder(Registry registry) {
            this.registry = registry;
        }

        public void Register<T>(T builder) where T : BindingBuilder {
            builders.Add(builder);
        }

        public Registry Build() {
            var bindings = builders
                .ToDictionary(x => x.Build(), x => x.InterfaceTypes.Concat(new[] { x.ImplementationType }).ToList());
            var registry = BuildRegistry(bindings);
            ValidateCircularDependencies(bindings, registry);
            return registry;
        }

        Registry BuildRegistry(Dictionary<Binding, List<Type>> bindings) {
            var data = bindings
                .SelectMany(x => x.Value.Select(t => (t, x: x.Key)))
                .GroupBy(x => x.t, x => x.x)
                .ToDictionary(x => x.Key, x => x.ToList());
            data = registry.Data
                .Concat(data)
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.SelectMany(x => x).ToList());
            return new Registry(data);
        }

        void ValidateCircularDependencies(Dictionary<Binding, List<Type>> bindings, Registry registry) {
            foreach (var binding in bindings) {
                var dependency = new DependencyInfo(binding.Key, binding.Value.Last());
                var dependencies = new Stack<DependencyInfo>();
                ValidateCircularDependencies(dependency, registry, dependencies);
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies) {
            ValidateCircularDependencies(current, dependencies);
            dependencies.Push(current);
            var info = InjectTypeInfoCache.GetOrBuild(current.DestType);
            ValidateCircularDependencies(current, registry, dependencies, info);
            dependencies.Pop();
        }

        void ValidateCircularDependencies(DependencyInfo current, Stack<DependencyInfo> dependencies) {
            foreach (var dependency in dependencies.Select((v, i) => (v, i))) {
                if (current.DestType != dependency.v.DestType) {
                    continue;
                }
                dependencies.Push(current);
                var messages = dependencies
                    .Take(dependency.i + 1)
                    .Select((v, i) => $"[{i}] `{v.DestType.FullName}` (at `{v}`)")
                    .ToList();
                messages.Insert(0, "Circular dependency was detected.");
                throw new SardinjectException(string.Join(Environment.NewLine, messages));
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies, InjectTypeInfo info) {
            ValidateCircularDependencies(current, registry, dependencies, info.Constructor);
            foreach (var field in info.Fields) {
                ValidateCircularDependencies(current, registry, dependencies, field);
            }
            foreach (var property in info.Properties) {
                ValidateCircularDependencies(current, registry, dependencies, property);
            }
            foreach (var method in info.Methods) {
                ValidateCircularDependencies(current, registry, dependencies, method);
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies, InjectConstructorInfo constructor) {
            foreach (var parameter in constructor.Parameters) {
                foreach (var binding in registry.GetBindings(parameter.ParameterInfo.ParameterType)) {
                    var dependency = new DependencyInfo(binding, parameter.ParameterInfo.ParameterType, current.Dest, current.DestType, constructor.ConstructorInfo);
                    ValidateCircularDependencies(dependency, registry, dependencies);
                }
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies, InjectFieldInfo field) {
            foreach (var binding in registry.GetBindings(field.FieldInfo.FieldType)) {
                var dependency = new DependencyInfo(binding, field.FieldInfo.FieldType, current.Dest, current.DestType, field.FieldInfo);
                ValidateCircularDependencies(dependency, registry, dependencies);
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies, InjectPropertyInfo property) {
            foreach (var binding in registry.GetBindings(property.PropertyInfo.PropertyType)) {
                var dependency = new DependencyInfo(binding, property.PropertyInfo.PropertyType, current.Dest, current.DestType, property.PropertyInfo);
                ValidateCircularDependencies(dependency, registry, dependencies);
            }
        }

        void ValidateCircularDependencies(DependencyInfo current, Registry registry, Stack<DependencyInfo> dependencies, InjectMethodInfo method) {
            foreach (var parameter in method.Parameters) {
                foreach (var binding in registry.GetBindings(parameter.ParameterInfo.ParameterType)) {
                    var dependency = new DependencyInfo(binding, parameter.ParameterInfo.ParameterType, current.Dest, current.DestType, method.MethodInfo);
                    ValidateCircularDependencies(dependency, registry, dependencies);
                }
            }
        }
    }
}
