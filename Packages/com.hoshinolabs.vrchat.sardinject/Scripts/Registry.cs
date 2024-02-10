using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class Registry {
        Dictionary<Type, Registration> table = new Dictionary<Type, Registration>();

        public Dictionary<Type, Registration> Table => table;

        internal Registry(Registration[] registrations) {
            foreach (var registration in registrations) {
                var types = registration.InterfaceTypes;
                types.Add(registration.ImplementationType);
                foreach (var type in types) {
                    table.Add(type, registration);
                }
            }
        }

        public bool TryGet(Type interfaceType, out Registration registration) {
            return table.TryGetValue(interfaceType, out registration);
        }
    }
}
