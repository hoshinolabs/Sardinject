using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class FindInstanceProvider : InstanceProviderBase {
        internal FindInstanceProvider() {

        }

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
                injector.Inject(instance, container, parameters);
            }
            return instance;
        }
    }
}
