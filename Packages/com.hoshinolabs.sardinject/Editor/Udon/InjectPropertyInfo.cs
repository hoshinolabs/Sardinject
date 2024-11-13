using System.Reflection;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class InjectPropertyInfo {
        public readonly PropertyInfo PropertyInfo;
        public readonly object Id;
        public readonly InjectMethodInfo Method;

        public InjectPropertyInfo(PropertyInfo propertyInfo, object id, InjectMethodInfo method) {
            PropertyInfo = propertyInfo;
            Id = id;
            Method = method;
        }
    }
}
