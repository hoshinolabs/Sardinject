using System;

namespace HoshinoLabs.Sardinject.Udon {
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : PreserveAttribute {
        public readonly object Id;

        public InjectAttribute(object id = null) {
            Id = id;
        }
    }
}
