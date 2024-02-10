using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Injector {
        InjectTypeInfo info;

        internal Injector(InjectTypeInfo info) {
            this.info = info;
        }

        public void Inject(object instance, Container container, Hashtable parameters) {
            InjectFields(instance, container, parameters);
            InjectProperties(instance, container, parameters);
            InjectMethods(instance, container, parameters);
        }

        void InjectFields(object instance, Container container, Hashtable parameters) {
            foreach (var field in info.Fields) {
                var value = container.ResolveWithParameters(field.FieldType, field.Name, parameters);
                field.SetValue(instance, value);
            }
        }

        void InjectProperties(object instance, Container container, Hashtable parameters) {
            foreach (var property in info.Properties) {
                var value = container.ResolveWithParameters(property.PropertyType, property.Name, parameters);
                property.SetValue(instance, value);
            }
        }

        void InjectMethods(object instance, Container container, Hashtable parameters) {
            foreach (var method in info.Methods) {
                var values = method.GetParameters()
                    .Select(x => container.ResolveWithParameters(x.ParameterType, x.Name, parameters))
                    .ToArray();
                method.Invoke(instance, values);
            }
        }
    }
}
