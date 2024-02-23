using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ContainerExtensions {
        public static T Resolve<T>(this Container self) {
            return (T)self.Resolve(typeof(T));
        }

        public static object ResolveWithParameters(this Container self, Type parameterType, string parameterName, Hashtable parameters) {
            if (parameters.ContainsKey(parameterType)) {
                return parameters[parameterType];
            }
            if (parameters.ContainsKey(parameterName)) {
                return parameters[parameterName];
            }
            return self.Resolve(parameterType);
        }
    }
}
