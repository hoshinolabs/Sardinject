using System.Collections;
using System.Collections.Generic;
using UdonSharp;
#if UNITY_EDITOR
using UdonSharpEditor;
#endif
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class NewGameObjectInstanceProvider : InstanceProviderBase {
        string gameObjectName;

        internal NewGameObjectInstanceProvider(string gameObjectName) {
            this.gameObjectName = gameObjectName;
        }

        public override bool IsRaw => false;
        public override bool IsPrefab => false;

        public override object[] GetInstance(Container container) {
            var name = string.IsNullOrEmpty(gameObjectName) ? implementationType.Name : gameObjectName;
            var go = new GameObject(name);
            go.SetActive(false);
            if (getTransform != null) {
                go.transform.SetParent(getTransform());
            }
#if UNITY_EDITOR
            var instance = typeof(UdonSharpBehaviour).IsAssignableFrom(implementationType)
                ? go.AddUdonSharpComponentEx(implementationType, false)
                : go.AddComponent(implementationType);
#else
            var instance = go.AddComponent(implementationType);
#endif
            injector.Inject(instance, container, id, parameters);
            instance.gameObject.SetActive(true);
            return new[] { instance };
        }
    }
}
