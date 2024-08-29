using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class RegistrationCache {
        List<Registration> cache = new List<Registration>();

        public IEnumerable<Registration> GetRegistrations() {
            return cache.ToArray();
        }

        public void Add(Registration registration) {
            cache.Add(registration);
        }
    }
}
