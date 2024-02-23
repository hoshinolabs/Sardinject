using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class InstanceProvider : InstanceProviderBase {
        Component instance;

        internal InstanceProvider(Component instance) {
            this.instance = instance;
        }

        public override bool IsRaw => true;
        public override bool IsPrefab => false;

        public override object GetInstance(Container container) {
            injector.Inject(instance, container, parameters);
            return instance;
        }
    }
}
