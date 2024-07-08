using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Injector {
        InjectTypeInfo info;

        internal Injector(InjectTypeInfo info) {
            this.info = info;
        }

        public void Inject(object instance, Container container, object id, Hashtable parameters) {
            InjectFields(instance, container, id, parameters);
            InjectProperties(instance, container, id, parameters);
            InjectMethods(instance, container, id, parameters);
        }

        void InjectFields(object instance, Container container, object id, Hashtable parameters) {
            foreach (var field in info.Fields) {
                InjectField(field, instance, container, id, parameters);
            }
        }

        void InjectField(FieldInfo field, object instance, Container container, object id, Hashtable parameters) {
            var attribute = field.GetCustomAttribute<InjectAttribute>();
            try {
                var value = Resolve(container, field.FieldType, field.Name, parameters);
                field.SetValue(instance, value);
            }
            catch (SardinjectException) {
                if (attribute.Optional) {
                    return;
                }
                throw SardinjectException.CreateUnableResolveField(field.FieldType, field.Name);
            }
        }

        void InjectProperties(object instance, Container container, object id, Hashtable parameters) {
            foreach (var property in info.Properties) {
                InjectProperty(property, instance, container, id, parameters);
            }
        }

        void InjectProperty(PropertyInfo property, object instance, Container container, object id, Hashtable parameters) {
            var attribute = property.GetCustomAttribute<InjectAttribute>();
            try {
                var value = Resolve(container, property.PropertyType, property.Name, parameters);
                property.SetValue(instance, value);
            }
            catch (SardinjectException) {
                if (attribute.Optional) {
                    return;
                }
                throw SardinjectException.CreateUnableResolveProperty(property.PropertyType, property.Name);
            }
        }

        void InjectMethods(object instance, Container container, object id, Hashtable parameters) {
            foreach (var method in info.Methods) {
                InjectMethod(method, instance, container, id, parameters);
            }
        }

        void InjectMethod(MethodInfo method, object instance, Container container, object id, Hashtable parameters) {
            var attribute = method.GetCustomAttribute<InjectAttribute>();
            try {
                var values = method.GetParameters()
                    .Select(x => Resolve(container, x.ParameterType, x.Name, parameters))
                    .ToArray();
                method.Invoke(instance, values);
            }
            catch (SardinjectException) {
                if (attribute.Optional) {
                    return;
                }
                throw SardinjectException.CreateUnableResolveMethod(method.Name);
            }
        }

        object Resolve(Container container, Type parameterType, string parameterName, Hashtable parameters) {
            if (parameters != null) {
                if (parameters.ContainsKey(parameterType)) {
                    return parameters[parameterType];
                }
                if (parameters.ContainsKey(parameterName)) {
                    return parameters[parameterName];
                }
            }
            return container.Resolve(parameterType);
        }
    }
}
