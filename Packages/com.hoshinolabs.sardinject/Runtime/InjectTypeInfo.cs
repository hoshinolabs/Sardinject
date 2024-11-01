using System;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectTypeInfo {
        public readonly Type Type;
        public readonly InjectConstructorInfo Constructor;
        public readonly InjectFieldInfo[] Fields;
        public readonly InjectPropertyInfo[] Properties;
        public readonly InjectMethodInfo[] Methods;

        public InjectTypeInfo(Type type, InjectConstructorInfo constructor, InjectFieldInfo[] fields, InjectPropertyInfo[] properties, InjectMethodInfo[] methods) {
            Type = type;
            Constructor = constructor;
            Fields = fields;
            Properties = properties;
            Methods = methods;
        }
    }
}
