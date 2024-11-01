using System;

namespace HoshinoLabs.Sardinject {
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : PreserveAttribute {
        public object Id { get; set; }

        public InjectAttribute() {

        }
    }
}
