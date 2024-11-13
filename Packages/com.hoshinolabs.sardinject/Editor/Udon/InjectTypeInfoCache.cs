using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoshinoLabs.Sardinject.Udon {
    internal static class InjectTypeInfoCache {
        static readonly Dictionary<Type, InjectTypeInfo> cache = new();

        public static InjectTypeInfo GetOrBuild(Sardinject.InjectTypeInfo typeInfo) {
            if (!cache.TryGetValue(typeInfo.Type, out var info)) {
                var fields = BuildFields(typeInfo);
                var properties = BuildProperties(typeInfo);
                var methods = BuildMethods(typeInfo);
                info = new InjectTypeInfo(typeInfo.Type, fields, properties, methods);
                cache.Add(typeInfo.Type, info);
            }
            return info;
        }

        static InjectFieldInfo[] BuildFields(Sardinject.InjectTypeInfo typeInfo) {
            var fields = typeInfo.Fields
                .Select(x => BuildField(x));
            return fields.ToArray();
        }

        static InjectFieldInfo BuildField(Sardinject.InjectFieldInfo fieldInfo) {
            return new InjectFieldInfo(fieldInfo.FieldInfo, fieldInfo.Id);
        }

        static InjectPropertyInfo[] BuildProperties(Sardinject.InjectTypeInfo typeInfo) {
            var properties = typeInfo.Properties
                .Select(x => BuildProperty(typeInfo, x));
            return properties.ToArray();
        }

        static InjectPropertyInfo BuildProperty(Sardinject.InjectTypeInfo typeInfo, Sardinject.InjectPropertyInfo propertyInfo) {
            var methodInfo = BuildMethod(propertyInfo.PropertyInfo.GetSetMethod());
            var method = BuildMethod(typeInfo, methodInfo);
            return new InjectPropertyInfo(propertyInfo.PropertyInfo, propertyInfo.Id, method);
        }

        static Sardinject.InjectMethodInfo BuildMethod(MethodInfo methodInfo) {
            var parameters = methodInfo.GetParameters()
                .Select(x => BuildParameter(x))
                .ToArray();
            return new Sardinject.InjectMethodInfo(methodInfo, parameters);
        }

        static Sardinject.InjectParameterInfo BuildParameter(ParameterInfo parameterInfo) {
            var attribute = parameterInfo.GetInjectAttribute();
            return new Sardinject.InjectParameterInfo(parameterInfo, attribute?.Id);
        }

        static InjectMethodInfo[] BuildMethods(Sardinject.InjectTypeInfo typeInfo) {
            var methods = typeInfo.Methods
                .Select(x => BuildMethod(typeInfo, x));
            return methods.ToArray();
        }

        static InjectMethodInfo BuildMethod(Sardinject.InjectTypeInfo typeInfo, Sardinject.InjectMethodInfo methodInfo) {
            var publicMethods = typeInfo.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var exportMethods = publicMethods
                .Where(x => x.Name == methodInfo.MethodInfo.Name)
                .Where(x => 0 < x.GetParameters().Length)
                .ToArray();
            var methodId = Array.IndexOf(exportMethods, methodInfo.MethodInfo);
            var methodSymbol = methodId < 0 ? methodInfo.MethodInfo.Name : $"__{methodId}_{methodInfo.MethodInfo.Name}";
            var parameters = methodInfo.Parameters
                .Select(x => BuildParameter(typeInfo, x))
                .ToArray();
            return new InjectMethodInfo(methodInfo.MethodInfo, methodSymbol, parameters);
        }

        static InjectParameterInfo BuildParameter(Sardinject.InjectTypeInfo typeInfo, Sardinject.InjectParameterInfo parameterInfo) {
            var publicMethods = typeInfo.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var exportParameters = publicMethods
                .SelectMany(x => x.GetParameters())
                .Where(x => x.Name == parameterInfo.ParameterInfo.Name)
                .ToArray();
            var parameterId = Array.IndexOf(exportParameters, parameterInfo.ParameterInfo);
            var parameterSymbol = $"__{parameterId}_{parameterInfo.ParameterInfo.Name}__param";
            return new InjectParameterInfo(parameterInfo.ParameterInfo, parameterSymbol, parameterInfo.Id);
        }
    }
}
