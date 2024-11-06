using System.Collections.Generic;
using System.Linq;
#if UDONSHARP
using UdonSharp;
#endif

namespace HoshinoLabs.Sardinject {
    public sealed class Injector {
        public readonly InjectTypeInfo TypeInfo;

        public Injector(InjectTypeInfo typeInfo) {
            TypeInfo = typeInfo;
        }

        public object Construct(Container container, IReadOnlyDictionary<object, IResolver> parameters) {
            var values = TypeInfo.Constructor.Parameters
                .Select(x => container.ResolveOrParameterOrId(x.ParameterInfo.Name, x.ParameterInfo.ParameterType, x.Id, parameters))
                .ToArray();
            var instance = TypeInfo.Constructor.ConstructorInfo.Invoke(values);
            Inject(instance, container, parameters);
            return instance;
        }

        public void Inject(object instance, Container container, IReadOnlyDictionary<object, IResolver> parameters) {
            InjectFields(instance, container, parameters);
            InjectProperties(instance, container, parameters);
            InjectMethods(instance, container, parameters);
#if UDONSHARP
            if (typeof(UdonSharpBehaviour).IsAssignableFrom(instance.GetType())) {
                UdonSharpBehaviourExtensions.ApplyProxyModifications((UdonSharpBehaviour)instance);
            }
#endif
        }

        void InjectFields(object instance, Container container, IReadOnlyDictionary<object, IResolver> parameters) {
            foreach (var field in TypeInfo.Fields) {
                var value = container.ResolveOrParameterOrId(field.FieldInfo.Name, field.FieldInfo.FieldType, field.Id, parameters);
                field.FieldInfo.SetValue(instance, value);
            }
        }

        void InjectProperties(object instance, Container container, IReadOnlyDictionary<object, IResolver> parameters) {
            foreach (var property in TypeInfo.Properties) {
                var value = container.ResolveOrParameterOrId(property.PropertyInfo.Name, property.PropertyInfo.PropertyType, property.Id, parameters);
                property.PropertyInfo.SetValue(instance, value);
            }
        }

        void InjectMethods(object instance, Container container, IReadOnlyDictionary<object, IResolver> parameters) {
            foreach (var method in TypeInfo.Methods) {
                var values = method.Parameters
                    .Select(x => container.ResolveOrParameterOrId(x.ParameterInfo.Name, x.ParameterInfo.ParameterType, x.Id, parameters))
                    .ToArray();
                method.MethodInfo.Invoke(instance, values);
            }
        }
    }
}
