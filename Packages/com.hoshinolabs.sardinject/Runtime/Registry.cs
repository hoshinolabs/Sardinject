using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Registry {
        Dictionary<Type, HashSet<Registration>> table = new Dictionary<Type, HashSet<Registration>>();
        public Dictionary<Type, HashSet<Registration>> Table => table;

        internal Registry(Registration[] registrations) {
            foreach(var registration in registrations) {
                var types = registration.InterfaceTypes
                    .Concat(new[] { registration.ImplementationType });
                foreach(var type in types) {
                    if (!table.TryGetValue(type, out var data)) {
                        data = new HashSet<Registration>();
                    }
                    data.Add(registration);
                    table[type] = data;
                }
            }
        }

        public bool Exists(Type interfaceType) {
            return table.ContainsKey(interfaceType);
        }

        public bool TryGet(Type interfaceType, out IEnumerable<Registration> registrations) {
            registrations = null;
            if (table.TryGetValue(interfaceType, out var _registrations)) {
                registrations = _registrations;
                return true;
            }
            return false;
        }
    }
}
