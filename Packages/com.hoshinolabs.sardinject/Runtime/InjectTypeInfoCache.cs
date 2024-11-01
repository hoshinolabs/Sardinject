using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal static class InjectTypeInfoCache {
        static readonly Dictionary<Type, InjectTypeInfo> cache = new();

        public static InjectTypeInfo GetOrBuild(Type type) {
            if (!cache.TryGetValue(type, out var info)) {
                var constructor = BuildConstructor(type);
                var fields = BuildFields(type);
                var properties = BuildProperties(type);
                var methods = BuildMethods(type);
                info = new InjectTypeInfo(type, constructor, fields, properties, methods);
                cache.Add(type, info);
            }
            return info;
        }

        static InjectConstructorInfo BuildConstructor(Type type) {
            var constructorInfos = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToArray();
            if (1 < constructorInfos.Count(x => x.IsDefined(typeof(InjectAttribute), false))) {
                throw new SardinjectException("Multiple constructors with the Inject attribute were found.");
            }
            var constructorInfo = constructorInfos
                .OrderBy(x => x.IsDefined(typeof(InjectAttribute), false))
                .ThenByDescending(x => x.GetParameters().Length)
                .First();
            var parameters = constructorInfo.GetParameters()
                .Select(x => BuildParameter(x))
                .ToArray();
            return new InjectConstructorInfo(constructorInfo, parameters);
        }

        static InjectFieldInfo[] BuildFields(Type type) {
            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false))
                .Select(x => BuildField(x));
            if (type.BaseType != null) {
                fields = fields.Concat(BuildFields(type.BaseType));
            }
            return fields.ToArray();
        }

        static InjectFieldInfo BuildField(FieldInfo fieldInfo) {
            var attribute = fieldInfo.GetCustomAttribute<InjectAttribute>();
            return new InjectFieldInfo(fieldInfo, attribute?.Id);
        }

        static InjectPropertyInfo[] BuildProperties(Type type) {
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false) && x.CanWrite)
                .Select(x => BuildProperty(x));
            if (type.BaseType != null) {
                properties = properties.Concat(BuildProperties(type.BaseType));
            }
            return properties.ToArray();
        }

        static InjectPropertyInfo BuildProperty(PropertyInfo propertyInfo) {
            var attribute = propertyInfo.GetCustomAttribute<InjectAttribute>();
            return new InjectPropertyInfo(propertyInfo, attribute?.Id);
        }

        static InjectMethodInfo[] BuildMethods(Type type) {
            var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false))
                .Select(x => BuildMethod(x));
            if (type.BaseType != null) {
                methods = methods.Concat(BuildMethods(type.BaseType));
            }
            return methods.ToArray();
        }

        static InjectMethodInfo BuildMethod(MethodInfo methodInfo) {
            var parameters = methodInfo.GetParameters()
                .Select(x => BuildParameter(x))
                .ToArray();
            return new InjectMethodInfo(methodInfo, parameters);
        }

        static InjectParameterInfo BuildParameter(ParameterInfo parameterInfo) {
            var attribute = parameterInfo.GetCustomAttribute<InjectAttribute>();
            return new InjectParameterInfo(parameterInfo, attribute?.Id);
        }
    }
}
