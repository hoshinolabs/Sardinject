using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class InjectAttribute : PreserveAttribute {
        public bool Optional { get; set; }
        public object Id { get; set; }

        public InjectAttribute() {

        }
    }
}
