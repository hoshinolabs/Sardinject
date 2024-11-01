using System;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonInjectTypeInfo {
        public readonly Type Type;
        public readonly UdonInjectFieldInfo[] Fields;
        public readonly UdonInjectPropertyInfo[] Properties;
        public readonly UdonInjectMethodInfo[] Methods;

        public UdonInjectTypeInfo(Type type, UdonInjectFieldInfo[] fields, UdonInjectPropertyInfo[] properties, UdonInjectMethodInfo[] methods) {
            Type = type;
            Fields = fields;
            Properties = properties;
            Methods = methods;
        }
    }
}
