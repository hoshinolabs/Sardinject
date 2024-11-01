using System.Reflection;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectPropertyInfo {
        public readonly PropertyInfo PropertyInfo;
        public readonly object Id;

        public InjectPropertyInfo(PropertyInfo propertyInfo, object id) {
            PropertyInfo = propertyInfo;
            Id = id;
        }
    }
}
