using System;
using System.Collections.Generic;
using System.Linq;

namespace HoshinoLabs.Sardinject {
    public sealed class Registry {
        public readonly IReadOnlyDictionary<Type, List<Binding>> Data;

        public Registry() {
            Data = new Dictionary<Type, List<Binding>>();
        }

        public Registry(IReadOnlyDictionary<Type, List<Binding>> data) {
            Data = data;
        }

        public IEnumerable<Binding> GetBindings(Type type) {
            if (Data.TryGetValue(type, out var bindings)) {
                return bindings;
            }
            return Enumerable.Empty<Binding>();
        }
    }
}
