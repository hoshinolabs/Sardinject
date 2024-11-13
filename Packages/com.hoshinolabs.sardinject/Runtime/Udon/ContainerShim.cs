using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.Udon;

namespace HoshinoLabs.Sardinject.Udon {
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    internal sealed partial class ContainerShim : Container {
        [Inject, SerializeField, HideInInspector]
        string[] _0;
        [Inject, SerializeField, HideInInspector]
        object[][] _1;
        [Inject, SerializeField, HideInInspector]
        int[][] _2;
        [Inject, SerializeField, HideInInspector]
        object[][] _3;
        [Inject, SerializeField, HideInInspector]
        int[][] _4;

        public override object Resolve(Type type) {
            return ResolveOrId(type, null);
        }

        internal object ResolveOrId(Type type, object id) {
            if (type.IsArray) {
                var elementType = type.GetElementType();
                return __ResolveOfArrayOrId(elementType, null);
            }
            return __ResolveOrId(type.FullName, id);
        }

        [RecursiveMethod]
        internal object ResolveOrId(string type, object id) {
            if (type.EndsWith("[]")) {
                var elementType = type.Substring(0, type.Length - 2);
                return __ResolveOfArrayOrId(elementType, null);
            }
            return __ResolveOrId(type, id);
        }

        [RecursiveMethod]
        object __ResolveOrId(string type, object id) {
            var idx = Array.IndexOf(_0, type);
            if (idx < 0) {
                Logger.LogError($"Unable to resolve for type `{type}`.");
                return null;
            }
            var __1 = _1[idx];
            for (var i = 0; i < __1.Length; i++) {
                if (id == null || id.Equals(__1[i])) {
                    return __Resolve(_2[idx][i]);
                }
            }
            Logger.LogError($"Unable to resolve for type `{type}`.");
            return null;
        }

        object __ResolveOfArrayOrId(Type type, object id) {
            var idx = Array.IndexOf(_0, type.FullName);
            if (idx < 0) {
                return Array.CreateInstance(type, 0);
            }
            var __3 = _3[idx];
            var arr = new object[__3.Length];
            var cnt = 0;
            for (var i = 0; i < __3.Length; i++) {
                if (id == null || id.Equals(__3[i])) {
                    arr[cnt++] = __Resolve(_4[idx][i]);
                }
            }
            var ret = Array.CreateInstance(type, cnt);
            Array.Copy(arr, ret, cnt);
            return ret;
        }

        [RecursiveMethod]
        object __ResolveOfArrayOrId(string type, object id) {
            var idx = Array.IndexOf(_0, type);
            if (idx < 0) {
                return Array.CreateInstance(typeof(UdonBehaviour), 0);
            }
            var __3 = _3[idx];
            var arr = new object[__3.Length];
            var cnt = 0;
            for (var i = 0; i < __3.Length; i++) {
                if (id == null || id.Equals(__3[i])) {
                    arr[cnt++] = __Resolve(_4[idx][i]);
                }
            }
            var ret = Array.CreateInstance(typeof(UdonBehaviour), cnt);
            Array.Copy(arr, ret, cnt);
            return ret;
        }

        internal object ResolveOrParameterOrId(string name, string type, object id, DataDictionary parameters) {
            if (parameters.TryGetValue(new DataToken(name), out var nameParameter)) {
                return nameParameter.Reference;
            }
            if (parameters.TryGetValue(new DataToken(type), out var typeParameter)) {
                return typeParameter.Reference;
            }
            return ResolveOrId(type, id);
        }

        public override void Inject(object instance) {
            __Inject(instance, new DataDictionary());
        }
    }
}
