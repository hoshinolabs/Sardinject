using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class ResolverCache {
        Dictionary<Registration, Lazy<object>> cache = new Dictionary<Registration, Lazy<object>>();

        public bool TryGet(Registration registration, out Lazy<object> value) {
            return cache.TryGetValue(registration, out value);
        }

        public Lazy<object> GetOrAdd(Registration registration, Container container) {
            if (cache.TryGetValue(registration, out var value)) {
                return value;
            }
            value = new(() => registration.GetInstance(container));
            cache.Add(registration, value);
            return value;
        }
    }
}
