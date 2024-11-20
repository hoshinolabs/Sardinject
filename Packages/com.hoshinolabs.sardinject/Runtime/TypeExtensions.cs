using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal static class TypeExtensions {
        public static bool IsEnumerable(this Type self) {
            return self.IsGenericType
                && self.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }
}
