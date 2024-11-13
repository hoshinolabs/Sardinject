using System.Reflection;

namespace HoshinoLabs.Sardinject.Udon {
    internal static class ParameterInfoExtensions {
        public static Sardinject.InjectAttribute GetInjectAttribute(this ParameterInfo self) {
            var attribute = self.GetCustomAttribute<Sardinject.InjectAttribute>();
            if (attribute != null) {
                return attribute;
            }
#if UDONSHARP
            var uattribute = self.GetCustomAttribute<InjectAttribute>();
            if (uattribute != null) {
                return new Sardinject.InjectAttribute(uattribute.Id);
            }
#endif
            return null;
        }
    }
}
