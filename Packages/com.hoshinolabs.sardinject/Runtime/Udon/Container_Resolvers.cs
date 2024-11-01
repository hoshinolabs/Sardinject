#if UDONSHARP
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed partial class Container {
        [SerializeField, HideInInspector]
        string[] _r0;
        [SerializeField, HideInInspector]
        object[][] _r1;

        DataDictionary _r_cache = new DataDictionary();
        object[] _r_args;
        object _r_ret;

        object __Resolve(int idx) {
            _r_args = _r1[idx];
            SendCustomEvent(_r0[idx]);
            return _r_ret;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectOverrideCachedScopeResolver() {
            var __0 = (int)_r_args[0];
            if (!_r_cache.TryGetValue(__0, out var value)) {
                var __1 = (int)_r_args[1];
                var __2 = (Container)_r_args[2];
                __2._r_args = _r1[__1];
                __2.SendCustomEvent(_r0[__1]);
                value = new DataToken(__2._r_ret);
                _r_cache.Add(__0, value);
            }
            _r_ret = value.Reference;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectOverrideSelfScopeResolver() {
            var __0 = (int)_r_args[0];
            if (!_r_cache.TryGetValue(__0, out var value)) {
                var __1 = (int)_r_args[1];
                _r_args = _r1[__1];
                SendCustomEvent(_r0[__1]);
                value = new DataToken(_r_ret);
                _r_cache.Add(__0, value);
            }
            _r_ret = value.Reference;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectUdonContainerResolver() {
            _r_ret = this;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectExistenceInstanceResolver() {
            var __0 = _r_args[0];
            var __1 = (int)_r_args[1];
            var __2 = (DataDictionary)_r_args[2];
            __Inject(__1, __0, __2);
            _r_ret = __0;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectExistenceComponentResolver() {
            var __0 = _r_args[0];
            var __1 = (int)_r_args[1];
            var __2 = (DataDictionary)_r_args[2];
            if (!typeof(Component).IsAssignableFrom(__0.GetType())) {
                Logger.LogError($"{__0.GetType()} is not a Component.");
                _r_ret = null;
                return;
            }
            var component = (Component)__0;
            __Inject(__1, component, __2);
            _r_ret = component;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectFindComponentResolver() {
            var __0 = _r_args[0];
            var __1 = (Transform)_r_args[1];
            var __2 = (int)_r_args[2];
            var __3 = (DataDictionary)_r_args[3];
            var component = __1
                ? __1.GetComponentInChildren(__0, true)
                : FindComponentInScene(__0);
            if (component == null) {
                Logger.LogError($"{__0} type component not found{(__1 == null ? "" : $" in {__1.name} children")}.");
                _r_ret = null;
                return;
            }
            __Inject(__2, component, __3);
            _r_ret = component;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectNewPrefabComponentResolver() {
            var __0 = _r_args[0];
            var __1 = (GameObject)_r_args[1];
            var __2 = (bool)_r_args[2];
            var __3 = (Transform)_r_args[3];
            var __4 = (int)_r_args[4];
            var __5 = (DataDictionary)_r_args[5];
            var instance = __3
                ? GameObject.Instantiate(__1, __3)
                : GameObject.Instantiate(__1);
            instance.name = __1.name;
            var component = instance.GetComponentInChildren(__0);
            if (component == null) {
                Logger.LogError($"{__0} type component not found{(__3 ? $" in {__3.name} children" : "")}.");
                _r_ret = null;
                return;
            }
            __Inject(__4, component, __5);
            if (__2) {
                instance.SetActive(__2);
            }
            _r_ret = component;
        }

        [RecursiveMethod]
        public void __HoshinoLabsSardinjectNewGameObjectComponentResolver() {
            var __0 = _r_args[0];
            var __1 = (GameObject)_r_args[1];
            var __2 = (Transform)_r_args[2];
            var __3 = (int)_r_args[3];
            var __4 = (DataDictionary)_r_args[4];
            var go = GameObject.Instantiate(__1);
            go.name = __1.name;
            if (__2 != null) {
                go.transform.SetParent(__2);
            }
            var component = go.GetComponent(__0);
            __Inject(__3, component, __4);
            go.SetActive(true);
            _r_ret = component;
        }
    }
}
#endif
