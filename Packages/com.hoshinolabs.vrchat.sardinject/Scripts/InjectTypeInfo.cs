using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal struct InjectTypeInfo {
        Type type;
        FieldInfo[] fields;
        PropertyInfo[] properties;
        MethodInfo[] methods;

        public Type Type => type;
        public FieldInfo[] Fields => fields;
        public PropertyInfo[] Properties => properties;
        public MethodInfo[] Methods => methods;

        internal InjectTypeInfo(Type type, FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods) {
            this.type = type;
            this.fields = fields;
            this.properties = properties;
            this.methods = methods;
        }
    }
}
