using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : PreserveAttribute {
        public InjectAttribute() {
            // TODO: validate generate and apply methods
        }
    }
}
