using System;

namespace HoshinoLabs.Sardinject {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PreserveAttribute : Attribute {
        public PreserveAttribute() {

        }
    }
}
