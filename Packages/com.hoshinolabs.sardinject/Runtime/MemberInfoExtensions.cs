using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal static class MemberInfoExtensions {
        public static bool IsInject(this MemberInfo self) {
            if (self.IsDefined(typeof(InjectAttribute), false)) {
                return true;
            }
#if UDONSHARP
            if (self.IsDefined(typeof(Udon.InjectAttribute), false)) {
                return true;
            }
#endif
            return false;
        }

        public static InjectAttribute GetInjectAttribute(this MemberInfo self) {
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
