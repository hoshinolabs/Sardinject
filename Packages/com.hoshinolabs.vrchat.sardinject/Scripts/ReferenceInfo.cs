using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal struct ReferenceInfo {
        Registration reference;
        Registration source;
        object member;

        public Registration Reference => reference;
        public Type ImplementationType => reference.ImplementationType;

        internal ReferenceInfo(Registration reference) {
            this.reference = reference;
            source = null;
            member = null;
        }
        internal ReferenceInfo(Registration reference, Registration source, MethodInfo method) {
            this.reference = reference;
            this.source = source;
            member = method;
        }
        internal ReferenceInfo(Registration reference, Registration source, FieldInfo field) {
            this.reference = reference;
            this.source = source;
            member = field;
        }
        internal ReferenceInfo(Registration reference, Registration source, PropertyInfo property) {
            this.reference = reference;
            this.source = source;
            member = property;
        }

        public override string ToString() {
            switch (member) {
                case MethodInfo method:
                    return $"{source.ImplementationType.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(x => x.Name))})";
                case FieldInfo field:
                    return $"{source.ImplementationType.FullName}.{field.Name}";
                case PropertyInfo property:
                    return $"{source.ImplementationType.FullName}.{property.Name}";
                default:
                    return string.Empty;
            }
        }
    }
}
