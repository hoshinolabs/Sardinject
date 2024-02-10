using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class ContainerProvider : InstanceProviderBase {
        internal ContainerProvider() {

        }

        public override bool IsRaw => true;
        public override bool IsPrefab => false;

        public override object GetInstance(Container container) {
            return container;
        }
    }
}
