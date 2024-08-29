using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class PrefabInstanceProvider : InstanceProviderBase {
        GameObject prefab;

        internal PrefabInstanceProvider(GameObject prefab) {
            this.prefab = prefab;
        }

        public override bool IsRaw => false;
        public override bool IsPrefab => true;

        public override object[] GetInstance(Container container) {
            var active = prefab.activeSelf;
            var transform = getTransform == null ? null : getTransform();
            prefab.SetActive(false);
            var instance = transform ? GameObject.Instantiate(prefab, transform) : GameObject.Instantiate(prefab);
            instance.name = prefab.name;
            var component = instance.GetComponent(implementationType);
            injector.Inject(component, container, id, parameters);
            prefab.SetActive(active);
            instance.SetActive(active);
            return new[] { instance };
        }
    }
}
