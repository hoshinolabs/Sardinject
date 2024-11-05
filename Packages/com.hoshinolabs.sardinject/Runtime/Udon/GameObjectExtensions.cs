using System;
using UdonSharp.Lib.Internal;
using UnityEngine;
using VRC.Udon;

namespace HoshinoLabs.Sardinject.Udon {
    internal static class GameObjectExtensions {
        public static Component GetComponent(this GameObject self, object type) {
            if (type.GetType() == typeof(Type)) {
                return self.GetComponent((Type)type);
            }
            var id = (string)type;
            foreach (var udon in self.GetComponents<UdonBehaviour>()) {
#if UNITY_EDITOR
                if (udon.GetProgramVariableType(CompilerConstants.UsbTypeNameHeapKey) == null) {
                    continue;
                }
#endif
                var value = udon.GetProgramVariable(CompilerConstants.UsbTypeNameHeapKey);
                if (value != null && (string)value == id) {
                    return udon;
                }
            }
            return null;
        }

        public static Component GetComponentInChildren(this GameObject self, object type, bool includeInactive) {
            if (type.GetType() == typeof(Type)) {
                return self.GetComponentInChildren((Type)type, includeInactive);
            }
            var id = (string)type;
            foreach (var udon in self.GetComponentsInChildren<UdonBehaviour>(includeInactive)) {
#if UNITY_EDITOR
                if (udon.GetProgramVariableType(CompilerConstants.UsbTypeNameHeapKey) == null) {
                    continue;
                }
#endif
                var value = udon.GetProgramVariable(CompilerConstants.UsbTypeNameHeapKey);
                if (value != null && (string)value == id) {
                    return udon;
                }
            }
            return null;
        }

        public static Component GetComponentInChildren(this GameObject self, object type) {
            if (type.GetType() == typeof(Type)) {
                return self.GetComponentInChildren((Type)type);
            }
            var id = (string)type;
            foreach (var udon in self.GetComponentsInChildren<UdonBehaviour>()) {
#if UNITY_EDITOR
                if (udon.GetProgramVariableType(CompilerConstants.UsbTypeNameHeapKey) == null) {
                    continue;
                }
#endif
                var value = udon.GetProgramVariable(CompilerConstants.UsbTypeNameHeapKey);
                if (value != null && (string)value == id) {
                    return udon;
                }
            }
            return null;
        }

//        public static Component GetComponentInParent(this GameObject self, object type) {
//            if (type.GetType() == typeof(Type)) {
//                return self.GetComponentInParent((Type)type);
//            }
//            var id = (long)type;
//            foreach (var udon in self.GetComponentsInParent<UdonBehaviour>()) {
//#if UNITY_EDITOR
//                if (udon.GetProgramVariableType(CompilerConstants.UsbTypeIDHeapKey) == null) {
//                    continue;
//                }
//#endif
//                var value = udon.GetProgramVariable(CompilerConstants.UsbTypeIDHeapKey);
//                if (value != null && (long)value == id) {
//                    return udon;
//                }
//            }
//            return null;
//        }
    }
}
