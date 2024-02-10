using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace HoshinoLabs.VRC.Sardinject.Udon {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("")]
    public class UdonContainer : UdonSharpBehaviour {
        [SerializeField, HideInInspector]
        string[] _0;
        [SerializeField, HideInInspector]
        int[] _1;
        [SerializeField, HideInInspector]
        int[] _2;
        [SerializeField, HideInInspector]
        GameObject[] _3;

        [SerializeField, HideInInspector]
        DataDictionary cache;

        public object Resolve(string type) {
            var idx = Array.IndexOf(_0, type);
            if (idx < 0) {
                Debug.LogError($"[<color=#47F1FF>Sardinject</color>] Unable to resolve for type `{type}`.");
                return null;
            }
            switch (_1[idx]) {
                case 0: {// Lifetime.Transient
                        var obj = _3[idx];
                        if (obj == null) {
                            return null;
                        }
                        return Instantiate(obj);
                    }
                case 1: {// Lifetime.Cached
                        var id = _2[idx];
                        if (!cache.TryGetValue(id, out var value)) {
                            var obj = _3[idx];
                            if (obj == null) {
                                return null;
                            }
                            value = Instantiate(obj);
                            cache.Add(id, value);
                        }
                        return value.Reference;
                    }
                case 2: {// Lifetime.Scoped
                        Debug.LogError($"[<color=#47F1FF>Sardinject</color>] Not implemented");
                        return null;
                    }
                default: {
                        var obj = _3[idx];
                        if (obj == null) {
                            return null;
                        }
                        return Instantiate(obj);
                    }
            }
        }

        public object Resolve(Type type) {
            return Resolve(type.ToString());
        }

        public void Inject(object instance) {
            Debug.LogError($"[<color=#47F1FF>Sardinject</color>] Not implemented");
        }
    }
}
