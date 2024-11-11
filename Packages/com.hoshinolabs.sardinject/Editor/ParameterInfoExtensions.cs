using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal static class ParameterInfoExtensions {
        public static InjectAttribute GetInjectAttribute(this ParameterInfo self) {
            var attribute = self.GetCustomAttribute<InjectAttribute>();
            if (attribute != null) {
                return attribute;
            }
#if UDONSHARP
            var uattribute = self.GetCustomAttribute<Udon.InjectAttribute>();
            if (uattribute != null) {
                return new InjectAttribute(uattribute.Id);
            }
#endif
            return null;
        }
    }
}
