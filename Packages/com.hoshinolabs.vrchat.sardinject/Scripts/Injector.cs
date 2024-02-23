using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
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
                try {
                    var value = container.ResolveWithParameters(field.FieldType, field.Name, parameters);
                    field.SetValue(instance, value);
                }
                catch(SardinjectException) {
                    throw SardinjectException.CreateUnableResolveField(field.FieldType, field.Name);
                }
            }
        }

        void InjectProperties(object instance, Container container, Hashtable parameters) {
            foreach (var property in info.Properties) {
                try {
                    var value = container.ResolveWithParameters(property.PropertyType, property.Name, parameters);
                    property.SetValue(instance, value);
                }
                catch (SardinjectException) {
                    throw SardinjectException.CreateUnableResolveProperty(property.PropertyType, property.Name);
                }
            }
        }

        void InjectMethods(object instance, Container container, Hashtable parameters) {
            foreach (var method in info.Methods) {
                try {
                    var values = method.GetParameters()
                        .Select(x => container.ResolveWithParameters(x.ParameterType, x.Name, parameters))
                        .ToArray();
                    method.Invoke(instance, values);
                }
                catch (SardinjectException) {
                    throw SardinjectException.CreateUnableResolveMethod(method.Name);
                }
            }
        }
    }
}
