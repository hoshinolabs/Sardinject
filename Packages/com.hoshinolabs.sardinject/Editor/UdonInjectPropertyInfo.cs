#if UDONSHARP
using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonInjectPropertyInfo {
        public readonly PropertyInfo PropertyInfo;
        public readonly object Id;
        public readonly UdonInjectMethodInfo Method;

        public UdonInjectPropertyInfo(PropertyInfo propertyInfo, object id, UdonInjectMethodInfo method) {
            PropertyInfo = propertyInfo;
            Id = id;
            Method = method;
        }
    }
}
#endif
