using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal static class RootContextHelper {
        //internal static Context Instance => instance;
        internal static Context Instance {
            get {
                var instance = typeof(RootContext).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
                return (Context)instance.GetValue(null);
            }
            set {
                var instance = typeof(RootContext).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
                instance.SetValue(null, value);
            }
        }
    }
}
