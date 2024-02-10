using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal static class InjectTypeInfoCache {
        // todo: DomainReloadが入らない場合正しく動くか要確認
        static Dictionary<Type, InjectTypeInfo> cache = new Dictionary<Type, InjectTypeInfo>();

        public static bool TryGet(Type type, out InjectTypeInfo value) {
            return cache.TryGetValue(type, out value);
        }

        public static InjectTypeInfo GetOrAdd(Type type) {
            if (cache.TryGetValue(type, out var info)) {
                return info;
            }
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false))
                .ToArray();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false))
                .ToArray();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false))
                .ToArray();
            info = new InjectTypeInfo(type, fields, properties, methods);
            cache.Add(type, info);
            return info;
        }

        public static InjectTypeInfo GetOrAdd<T>() {
            return GetOrAdd(typeof(T));
        }
    }
}
