using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class Registry {
        Dictionary<Type, Registration> table = new Dictionary<Type, Registration>();

        public Dictionary<Type, Registration> Table => table;

        internal Registry(Registration[] registrations) {
            foreach (var registration in registrations) {
                foreach (var interfaceType in registration.InterfaceTypes) {
                    table.Add(interfaceType, registration);
                }
                if (!table.ContainsKey(registration.ImplementationType)) {
                    table.Add(registration.ImplementationType, registration);
                }
            }
        }

        public bool Exists(Type interfaceType) {
            return table.ContainsKey(interfaceType);
        }

        public bool TryGet(Type interfaceType, out Registration registration) {
            return table.TryGetValue(interfaceType, out registration);
        }
    }
}
