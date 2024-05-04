using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class FindInstanceProvider : InstanceProviderBase {
        public override bool IsRaw => true;
        public override bool IsPrefab => false;

        public override object GetInstance(Container container) {
            var instance = (Component)null;
            if (getTransform == null) {
                instance = (Component)GameObject.FindObjectOfType(implementationType, true);
            }
            else {
                var transform = getTransform();
                instance = transform.GetComponentInChildren(implementationType, true);
            }
            if (instance != null) {
                injector.Inject(instance, container, id, parameters);
            }
            return instance;
        }
    }
}
