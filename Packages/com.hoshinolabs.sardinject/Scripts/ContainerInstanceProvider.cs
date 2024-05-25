using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class ContainerInstanceProvider : InstanceProviderBase {
        public override bool IsRaw => true;
        public override bool IsPrefab => false;

        public override object GetInstance(Container container) {
            return container;
        }
    }
}
