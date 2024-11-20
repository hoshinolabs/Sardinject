using System;
using System.Collections.Generic;
using System.Linq;

namespace HoshinoLabs.Sardinject {
    public sealed class Registry {
        public readonly IReadOnlyDictionary<Type, List<Binding>> Bindings;

        public Registry() {
            Bindings = new Dictionary<Type, List<Binding>>();
        }

        public Registry(IReadOnlyDictionary<Type, List<Binding>> bindings) {
            Bindings = bindings;
        }

        public IEnumerable<Binding> GetBindings(Type type) {
            if (type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                return Bindings
                    .Where(x => x.Key == type || x.Key == genericType)
                    .SelectMany(x => x.Value);
            }
            return Bindings.GetValueOrDefault(type, new());
        }
    }
}
