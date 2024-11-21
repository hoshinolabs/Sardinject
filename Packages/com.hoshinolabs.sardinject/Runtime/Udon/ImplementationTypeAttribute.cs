using System;

namespace HoshinoLabs.Sardinject.Udon {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ImplementationTypeAttribute : Attribute {
        public readonly Type ImplementationType;

        public ImplementationTypeAttribute(Type implementationType) {
            ImplementationType = implementationType;
        }
    }
}
