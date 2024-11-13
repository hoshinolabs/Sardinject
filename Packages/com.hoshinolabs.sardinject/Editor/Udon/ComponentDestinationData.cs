using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class ComponentDestinationData {
        public readonly Transform Transform;

        public ComponentDestinationData(Transform transform) {
            Transform = transform;
        }
    }
}
