using System;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class InjectTypeInfo {
        public readonly Type Type;
        public readonly InjectFieldInfo[] Fields;
        public readonly InjectPropertyInfo[] Properties;
        public readonly InjectMethodInfo[] Methods;

        public InjectTypeInfo(Type type, InjectFieldInfo[] fields, InjectPropertyInfo[] properties, InjectMethodInfo[] methods) {
            Type = type;
            Fields = fields;
            Properties = properties;
            Methods = methods;
        }
    }
}
