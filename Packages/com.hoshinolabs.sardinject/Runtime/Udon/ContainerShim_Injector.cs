using System;
using UdonSharp;
using UdonSharp.Lib.Internal;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.Udon;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed partial class ContainerShim {
        [Inject, SerializeField, HideInInspector]
        string[] _i0;
        [Inject, SerializeField, HideInInspector]
        string[][] _i1;
        [Inject, SerializeField, HideInInspector]
        string[][] _i2;
        [Inject, SerializeField, HideInInspector]
        object[][] _i3;
        [Inject, SerializeField, HideInInspector]
        string[][] _i4;
        [Inject, SerializeField, HideInInspector]
        string[][] _i5;
        [Inject, SerializeField, HideInInspector]
        object[][] _i6;
        [Inject, SerializeField, HideInInspector]
        string[][] _i7;
        [Inject, SerializeField, HideInInspector]
        string[][] _i8;
        [Inject, SerializeField, HideInInspector]
        string[][] _i9;
        [Inject, SerializeField, HideInInspector]
        string[][][] _i10;
        [Inject, SerializeField, HideInInspector]
        string[][][] _i11;
        [Inject, SerializeField, HideInInspector]
        object[][][] _i12;
        [Inject, SerializeField, HideInInspector]
        string[][] _i13;
        [Inject, SerializeField, HideInInspector]
        string[][][] _i14;

        void __Inject(object instance, DataDictionary parameters) {
            if (instance.GetType() != typeof(UdonBehaviour)) {
                return;
            }
            var udon = (UdonBehaviour)instance;
#if UNITY_EDITOR
            if (udon.GetProgramVariableType(CompilerConstants.UsbTypeIDHeapKey) == null) {
                return;
            }
#endif
            var value = udon.GetProgramVariable(CompilerConstants.UsbTypeNameHeapKey);
            if(value == null) {
                return;
            }

            var id = (string)value;
            var idx = Array.IndexOf(_i0, id);
            if (idx < 0) {
                return;
            }

            __Inject(idx, udon, parameters);
        }

        void __Inject(int idx, object instance, DataDictionary parameters) {
            if (instance.GetType() != typeof(UdonBehaviour)) {
                return;
            }
            var udon = (UdonBehaviour)instance;
#if UNITY_EDITOR
            if (udon.GetProgramVariableType(CompilerConstants.UsbTypeIDHeapKey) == null) {
                return;
            }
#endif

            __Inject(idx, udon, parameters);
        }

        [RecursiveMethod]
        void __Inject(int idx, UdonBehaviour instance, DataDictionary parameters) {
            var __1 = _i1[idx];
            var __2 = _i2[idx];
            var __3 = _i3[idx];
            for (var i = 0; i < __1.Length; i++) {
                var ___1 = __1[i];
                var value = ResolveOrParameterOrId(___1, __2[i], __3[i], parameters);
                instance.SetProgramVariable(___1, value);
            }
            var __4 = _i4[idx];
            var __5 = _i5[idx];
            var __6 = _i6[idx];
            var __7 = _i7[idx];
            var __8 = _i8[idx];
            for (var i = 0; i < __4.Length; i++) {
                var value = ResolveOrParameterOrId(__4[i], __5[i], __6[i], parameters);
                instance.SetProgramVariable(__8[i], value);
                instance.SendCustomEvent(__7[i]);
            }
            var __9 = _i9[idx];
            var __10 = _i10[idx];
            var __11 = _i11[idx];
            var __12 = _i12[idx];
            var __13 = _i13[idx];
            var __14 = _i14[idx];
            for (var i = 0; i < __9.Length; i++) {
                var ___10 = __10[i];
                var ___11 = __11[i];
                var ___12 = __12[i];
                var ___14 = __14[i];
                for (var j = 0; j < ___10.Length; j++) {
                    var value = ResolveOrParameterOrId(___10[j], ___11[j], ___12[j], parameters);
                    instance.SetProgramVariable(___14[j], value);
                }
                instance.SendCustomEvent(__13[i]);
            }
        }
    }
}
