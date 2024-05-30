using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class FindInstanceProvider : InstanceProviderBase {
        public override bool IsRaw => true;
        public override bool IsPrefab => false;

        public override object[] GetInstance(Container container) {
            var transform = getTransform == null ? null : getTransform();
            var instances = transform == null
                ? GameObject.FindObjectsOfType(implementationType, true)
                : transform.GetComponentsInChildren(implementationType, true);
            foreach (var instance in instances) {
                injector.Inject(instance, container, id, parameters);
            }
            return instances;
        }
    }
}
