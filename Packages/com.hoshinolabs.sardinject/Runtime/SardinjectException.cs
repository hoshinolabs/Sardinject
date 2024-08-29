using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class SardinjectException : Exception {
        public SardinjectException(string message) : base(message) {

        }

        internal static SardinjectException CreateUnableResolve(Type type) {
            return new SardinjectException($"Unable to resolve for type `{type}`.");
        }

        internal static SardinjectException CreateUnableResolveField(Type type, string name) {
            return new SardinjectException($"Unable to resolve field for type `{type}` with name `{name}`.");
        }

        internal static SardinjectException CreateUnableResolveProperty(Type type, string name) {
            return new SardinjectException($"Unable to resolve property for type `{type}` with name `{name}`.");
        }

        internal static SardinjectException CreateUnableResolveMethod(string name) {
            return new SardinjectException($"Unable to resolve method for name `{name}`.");
        }

        internal static SardinjectException CreateCircularDependency(IEnumerable<ReferenceInfo> stack) {
            var messages = stack
                .Reverse()
                .Select((v, i) => $"\tat `{v}` type of `{v.ImplementationType.FullName}`")
                .ToList();
            messages.Insert(0, "Circular dependency was detected.");
            return new SardinjectException(string.Join(Environment.NewLine, messages));
        }

        internal static SardinjectException CreateNotAssignableFrom(Type implementationType, Type interfaceType) {
            return new SardinjectException($"`{implementationType}` is not assingnable from `{interfaceType}`.");
        }
    }
}
