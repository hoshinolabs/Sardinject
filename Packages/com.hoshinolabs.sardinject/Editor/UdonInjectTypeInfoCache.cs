#if UDONSHARP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal static class UdonInjectTypeInfoCache {
        static readonly Dictionary<Type, UdonInjectTypeInfo> cache = new();

        public static UdonInjectTypeInfo GetOrBuild(InjectTypeInfo typeInfo) {
            if (!cache.TryGetValue(typeInfo.Type, out var info)) {
                var fields = BuildFields(typeInfo);
                var properties = BuildProperties(typeInfo);
                var methods = BuildMethods(typeInfo);
                info = new UdonInjectTypeInfo(typeInfo.Type, fields, properties, methods);
                cache.Add(typeInfo.Type, info);
            }
            return info;
        }

        static UdonInjectFieldInfo[] BuildFields(InjectTypeInfo typeInfo) {
            var fields = typeInfo.Fields
                .Select(x => BuildField(x));
            return fields.ToArray();
        }

        static UdonInjectFieldInfo BuildField(InjectFieldInfo fieldInfo) {
            return new UdonInjectFieldInfo(fieldInfo.FieldInfo, fieldInfo.Id);
        }

        static UdonInjectPropertyInfo[] BuildProperties(InjectTypeInfo typeInfo) {
            var properties = typeInfo.Properties
                .Select(x => BuildProperty(typeInfo, x));
            return properties.ToArray();
        }

        static UdonInjectPropertyInfo BuildProperty(InjectTypeInfo typeInfo, InjectPropertyInfo propertyInfo) {
            var methodInfo = BuildMethod(propertyInfo.PropertyInfo.GetSetMethod());
            var method = BuildMethod(typeInfo, methodInfo);
            return new UdonInjectPropertyInfo(propertyInfo.PropertyInfo, propertyInfo.Id, method);
        }

        static InjectMethodInfo BuildMethod(MethodInfo methodInfo) {
            var parameters = methodInfo.GetParameters()
                .Select(x => BuildParameter(x))
                .ToArray();
            return new InjectMethodInfo(methodInfo, parameters);
        }

        static InjectParameterInfo BuildParameter(ParameterInfo parameterInfo) {
            var attribute = parameterInfo.GetInjectAttribute();
            return new InjectParameterInfo(parameterInfo, attribute?.Id);
        }

        static UdonInjectMethodInfo[] BuildMethods(InjectTypeInfo typeInfo) {
            var methods = typeInfo.Methods
                .Select(x => BuildMethod(typeInfo, x));
            return methods.ToArray();
        }

        static UdonInjectMethodInfo BuildMethod(InjectTypeInfo typeInfo, InjectMethodInfo methodInfo) {
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
            return new UdonInjectMethodInfo(methodInfo.MethodInfo, methodSymbol, parameters);
        }

        static UdonInjectParameterInfo BuildParameter(InjectTypeInfo typeInfo, InjectParameterInfo parameterInfo) {
            var publicMethods = typeInfo.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var exportParameters = publicMethods
                .SelectMany(x => x.GetParameters())
                .Where(x => x.Name == parameterInfo.ParameterInfo.Name)
                .ToArray();
            var parameterId = Array.IndexOf(exportParameters, parameterInfo.ParameterInfo);
            var parameterSymbol = $"__{parameterId}_{parameterInfo.ParameterInfo.Name}__param";
            return new UdonInjectParameterInfo(parameterInfo.ParameterInfo, parameterSymbol, parameterInfo.Id);
        }
    }
}
#endif
