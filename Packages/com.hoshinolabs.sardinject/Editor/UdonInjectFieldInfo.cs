using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonInjectFieldInfo {
        public readonly FieldInfo FieldInfo;
        public readonly object Id;

        public UdonInjectFieldInfo(FieldInfo fieldInfo, object id) {
            FieldInfo = fieldInfo;
            Id = id;
        }
    }
}
