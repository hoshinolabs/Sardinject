using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class NewGameObjectInstanceProvider : InstanceProviderBase {
        string gameObjectName;

        internal NewGameObjectInstanceProvider(string gameObjectName) {
            this.gameObjectName = gameObjectName;
        }

        public override bool IsRaw => false;
        public override bool IsPrefab => false;

        public override object GetInstance(Container container) {
            var name = string.IsNullOrEmpty(gameObjectName) ? implementationType.Name : gameObjectName;
            var go = new GameObject(name);
            go.SetActive(false);
            if(getTransform != null) {
                go.transform.SetParent(getTransform());
            }
            var instance = go.AddComponent(implementationType);
            injector.Inject(instance, container, parameters);
            instance.gameObject.SetActive(true);
            return instance;
        }
    }
}
