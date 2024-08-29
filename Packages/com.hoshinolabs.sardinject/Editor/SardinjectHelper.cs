using HoshinoLabs.Sardinject.Udon;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal static class SardinjectHelper {
        public static GameObject Go { get; set; }
        public static Dictionary<Container, IContainer> ContainerCache { get; set; }

        public static Context Context {
            get {
                var instance = typeof(ProjectContext).GetField("context", BindingFlags.Static | BindingFlags.NonPublic);
                return (Context)instance.GetValue(null);
            }
            set {
                var instance = typeof(ProjectContext).GetField("context", BindingFlags.Static | BindingFlags.NonPublic);
                instance.SetValue(null, value);
            }
        }
    }
}
