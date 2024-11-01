using System;
using System.Linq;
using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal struct DependencyInfo {
        public readonly Binding Dest;
        public readonly Type DestType;
        public readonly Binding Src;
        public readonly Type SrcType;
        public readonly object Member;

        public DependencyInfo(Binding dest, Type destType) {
            Dest = dest;
            DestType = destType;
            Src = null;
            SrcType = null;
            Member = null;
        }

        public DependencyInfo(Binding dest, Type destType, Binding src, Type srcType, FieldInfo field) {
            Dest = dest;
            DestType = destType;
            Src = src;
            SrcType = srcType;
            Member = field;
        }

        public DependencyInfo(Binding dest, Type destType, Binding src, Type srcType, PropertyInfo property) {
            Dest = dest;
            DestType = destType;
            Src = src;
            SrcType = srcType;
            Member = property;
        }

        public DependencyInfo(Binding dest, Type destType, Binding src, Type srcType, MethodBase method) {
            Dest = dest;
            DestType = destType;
            Src = src;
            SrcType = srcType;
            Member = method;
        }

        public override string ToString() {
            switch (Member) {
                case ConstructorInfo constructor: {
                        return $"{SrcType}..ctor({string.Join(", ", constructor.GetParameters().Select(x => x.Name))})";
                    }
                case MethodInfo method: {
                        return $"{SrcType.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(x => x.Name))})";
                    }
                case FieldInfo field: {
                        return $"{SrcType.FullName}.{field.Name}";
                    }
                case PropertyInfo property: {
                        return $"{SrcType.FullName}.{property.Name}";
                    }
            }
            return string.Empty;
        }
    }
}
