using System.Reflection;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectFieldInfo {
        public readonly FieldInfo FieldInfo;
        public readonly object Id;

        public InjectFieldInfo(FieldInfo fieldInfo, object id) {
            FieldInfo = fieldInfo;
            Id = id;
        }
    }
}
