using System;
using System.Collections;
using System.Linq;

namespace HoshinoLabs.Sardinject {
    internal static class EnumerableExtensions {
        public static IEnumerable Cast(this IEnumerable self, Type elementType) {
            var method = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(elementType);
            return (IEnumerable)method.Invoke(null, new object[] { self });
        }

        public static Array ToArray(this IEnumerable self, Type elementType) {
            var method = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(elementType);
            return (Array)method.Invoke(null, new object[] { self });
        }
    }
}
