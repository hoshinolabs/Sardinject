using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed partial class ContainerShim {
        [Inject, SerializeField, HideInInspector]
        GameObject[] _u0;

        Component FindComponentInScene(object type) {
            foreach (var go in _u0) {
                var component = go.GetComponentInChildren(type, true);
                if (component == null) {
                    continue;
                }
                return component;
            }
            return null;
        }
    }
}
