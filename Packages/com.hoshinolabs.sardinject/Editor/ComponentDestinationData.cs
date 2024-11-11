#if UDONSHARP
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class ComponentDestinationData {
        public readonly Transform Transform;

        public ComponentDestinationData(Transform transform) {
            Transform = transform;
        }
    }
}
#endif
