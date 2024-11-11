#if UDONSHARP
using System;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Data;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonContainerResolver : IResolver {
        public readonly ComponentDestination Destination;

        GameObject rootGo;

        List<ResolverData> resolverDatas = new();
        List<UdonInjectTypeInfo> typeInfos = new();

        public UdonContainerResolver(ComponentDestination destination) {
            Destination = destination;
        }

        bool TryBuildResolverParameterKey(object key, out string result) {
            switch (key) {
                case string _key: {
                        result = _key;
                        return true;
                    }
                case Type _key: {
                        result = _key.FullName;
                        return true;
                    }
            }
            Logger.LogWarning($"Parameter key type {key.GetType()} is not specified.");
            result = null;
            return false;
        }

        bool TryBuildResolverParameterValue(IResolver resolver, out object result) {
            switch (resolver) {
                case ParameterResolver _resolver: {
                        result = typeof(UdonSharpBehaviour).IsAssignableFrom(_resolver.Value.GetType())
                            ? UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)_resolver.Value)
                            : _resolver.Value;
                        return true;
                    }
            }
            Logger.LogWarning($"Parameter value type {resolver.GetType()} is not specified.");
            result = null;
            return false;
        }

        DataDictionary BuildResolverParameters(IReadOnlyDictionary<object, IResolver> parameters) {
            var result = new DataDictionary();
            foreach (var parameter in parameters) {
                if (!TryBuildResolverParameterKey(parameter.Key, out var key)) {
                    continue;
                }
                if (!TryBuildResolverParameterValue(parameter.Value, out var value)) {
                    continue;
                }
                result.Add(key, new(value));
            }
            return result;
        }

        bool TryBuildComponentDestinationTransform(IResolver resolver, out Transform result) {
            switch (resolver) {
                case ParameterResolver _resolver: {
                        result = (Transform)_resolver.Value;
                        return true;
                    }
            }
            Logger.LogWarning($"Transform value type {resolver.GetType()} is not specified.");
            result = null;
            return false;
        }

        ComponentDestinationData BuildComponentDestinationData(ComponentDestination destination) {
            TryBuildComponentDestinationTransform(destination.Transform, out var transform);
            return new ComponentDestinationData(transform);
        }

        int AddOrBuildInjectTypeInfo(InjectTypeInfo typeInfo) {
            var idx = typeInfos.FindIndex(x => x.Type == typeInfo.Type);
            if (idx < 0) {
                var info = UdonInjectTypeInfoCache.GetOrBuild(typeInfo);
                typeInfos.Add(info);
                idx = typeInfos.Count() - 1;
            }
            return idx;
        }

        ResolverData BuildResolverData(OverrideCachedScopeResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var resolverId = resolver.Resolver.GetHashCode();
            var resolverIdx = AddOrBuildResolverData(resolver.Resolver, container, ucontainer);
            var resolverContainer = resolver.Container == container ? ucontainer : resolver.Container.Resolve<Udon.Container>();
            var args = new object[] {
                resolverId,
                resolverIdx,
                UdonSharpEditorUtility.GetBackingUdonBehaviour(resolverContainer)
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(OverrideSelfScopeResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var resolverId = resolver.Resolver.GetHashCode();
            var resolverIdx = AddOrBuildResolverData(resolver.Resolver, container, ucontainer);
            var args = new object[] {
                resolverId,
                resolverIdx
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(ContainerResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            return new ResolverData(signature, null);
        }

        ResolverData BuildResolverData(UdonContainerResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            return new ResolverData(signature, null);
        }

        ResolverData BuildResolverData(ExistenceInstanceResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var instance = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.Instance.GetType())
                ? UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)resolver.Instance)
                : resolver.Instance;
            var injectorIdx = AddOrBuildInjectTypeInfo(resolver.Injector.TypeInfo);
            var parameters = BuildResolverParameters(resolver.Parameters);
            var args = new object[] {
                instance,
                injectorIdx,
                parameters
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(ExistenceComponentResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var instance = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.Instance.GetType())
                ? UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)resolver.Instance)
                : resolver.Instance;
            var injectorIdx = AddOrBuildInjectTypeInfo(resolver.Injector.TypeInfo);
            var parameters = BuildResolverParameters(resolver.Parameters);
            var args = new object[] {
                instance,
                injectorIdx,
                parameters
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(FindComponentResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var componentType = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.ComponentType)
                ? (object)resolver.ComponentType.FullName
                : (object)resolver.ComponentType;
            var destination = BuildComponentDestinationData(resolver.Destination);
            var injectorIdx = AddOrBuildInjectTypeInfo(resolver.Injector.TypeInfo);
            var parameters = BuildResolverParameters(resolver.Parameters);
            var args = new object[] {
                componentType,
                destination.Transform,
                injectorIdx,
                parameters
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(NewPrefabComponentResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var componentType = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.ComponentType)
                ? (object)resolver.ComponentType.FullName
                : (object)resolver.ComponentType;
            var active = resolver.Prefab.activeSelf;
            if (active) {
                resolver.Prefab.SetActive(false);
            }
            var prefab = GameObject.Instantiate(resolver.Prefab, rootGo.transform);
            if (active) {
                resolver.Prefab.SetActive(active);
            }
            var destination = BuildComponentDestinationData(resolver.Destination);
            var injectorIdx = AddOrBuildInjectTypeInfo(resolver.Injector.TypeInfo);
            var parameters = BuildResolverParameters(resolver.Parameters);
            var args = new object[] {
                componentType,
                prefab,
                active,
                destination.Transform,
                injectorIdx,
                parameters
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(NewGameObjectComponentResolver resolver, Container container, Udon.Container ucontainer) {
            var signature = $"__{resolver.GetType().FullName.Replace(".", "")}";
            var componentType = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.ComponentType)
                ? (object)resolver.ComponentType.FullName
                : (object)resolver.ComponentType;
            var gameObjectName = string.IsNullOrEmpty(resolver.GameObjectName)
                ? resolver.ComponentType.Name
                : resolver.GameObjectName;
            var prefab = new GameObject(gameObjectName);
            prefab.SetActive(false);
            prefab.transform.SetParent(rootGo.transform);
            var _ = typeof(UdonSharpBehaviour).IsAssignableFrom(resolver.ComponentType)
                ? prefab.AddUdonSharpComponent(resolver.ComponentType, false)
                : prefab.AddComponent(resolver.ComponentType);
            var destination = BuildComponentDestinationData(resolver.Destination);
            var injectorIdx = AddOrBuildInjectTypeInfo(resolver.Injector.TypeInfo);
            var parameters = BuildResolverParameters(resolver.Parameters);
            var args = new object[] {
                componentType,
                prefab,
                destination.Transform,
                injectorIdx,
                parameters
            };
            return new ResolverData(signature, args);
        }

        ResolverData BuildResolverData(IResolver resolver, Container container, Udon.Container ucontainer) {
            switch (resolver) {
                case OverrideCachedScopeResolver overrideCachedScopeResolver: {
                        return BuildResolverData(overrideCachedScopeResolver, container, ucontainer);
                    }
                case OverrideSelfScopeResolver overrideSelfScopeResolver: {
                        return BuildResolverData(overrideSelfScopeResolver, container, ucontainer);
                    }
                case ContainerResolver containerResolver: {
                        return BuildResolverData(containerResolver, container, ucontainer);
                    }
                case UdonContainerResolver udonContainerResolver: {
                        return BuildResolverData(udonContainerResolver, container, ucontainer);
                    }
                case ExistenceInstanceResolver existenceInstanceResolver: {
                        return BuildResolverData(existenceInstanceResolver, container, ucontainer);
                    }
                case ExistenceComponentResolver existenceComponentResolver: {
                        return BuildResolverData(existenceComponentResolver, container, ucontainer);
                    }
                case FindComponentResolver findComponentResolver: {
                        return BuildResolverData(findComponentResolver, container, ucontainer);
                    }
                case NewPrefabComponentResolver newPrefabComponentResolver: {
                        return BuildResolverData(newPrefabComponentResolver, container, ucontainer);
                    }
                case NewGameObjectComponentResolver newGameObjectComponentResolver: {
                        return BuildResolverData(newGameObjectComponentResolver, container, ucontainer);
                    }
            }
            Logger.LogWarning($"Resolver type {resolver.GetType()} is not supported.");
            return new ResolverData(null, null);
        }

        int AddOrBuildResolverData(IResolver resolver, Container container, Udon.Container ucontainer) {
            resolverDatas.Add(BuildResolverData(resolver, container, ucontainer));
            return resolverDatas.Count() - 1;
        }

        (string[] _0, object[][] _1, int[][] _2, object[][] _3, int[][] _4) BuildContainerData(Container container, Udon.Container ucontainer) {
            var containerData = container.Registry.Data
                .ToDictionary(x => x.Key.FullName, x => {
                    return x.Value.Select(x => {
                        var resolverIdx = AddOrBuildResolverData(x.Resolver, container, ucontainer);
                        return (
                            _0: x.Id,
                            _1: resolverIdx
                        );
                    });
                });
            return (
                _0: containerData.Keys.ToArray(),
                _1: containerData.Values.Select(x => x.Select(x => x._0).Reverse().ToArray()).ToArray(),
                _2: containerData.Values.Select(x => x.Select(x => x._1).Reverse().ToArray()).ToArray(),
                _3: containerData.Values.Select(x => x.Select(x => x._0).ToArray()).ToArray(),
                _4: containerData.Values.Select(x => x.Select(x => x._1).ToArray()).ToArray()
            );
        }

        (string[] _0, object[][] _1) BuildResolverData() {
            return (
                _0: resolverDatas.Select(x => x.Signature).ToArray(),
                _1: resolverDatas.Select(x => x.Args).ToArray()
            );
        }

        (string[] _0, string[][] _1, string[][] _2, object[][] _3, string[][] _4, string[][] _5, object[][] _6, string[][] _7, string[][] _8, string[][] _9, string[][][] _10, string[][][] _11, object[][][] _12, string[][] _13, string[][][] _14) BuildTypeInfoData() {
            return (
                _0: typeInfos.Select(x => x.Type.FullName).ToArray(),
                _1: typeInfos.Select(x => x.Fields.Select(x => x.FieldInfo.Name).ToArray()).ToArray(),
                _2: typeInfos.Select(x => x.Fields.Select(x => x.FieldInfo.FieldType.FullName).ToArray()).ToArray(),
                _3: typeInfos.Select(x => x.Fields.Select(x => x.Id).ToArray()).ToArray(),
                _4: typeInfos.Select(x => x.Properties.Select(x => x.PropertyInfo.Name).ToArray()).ToArray(),
                _5: typeInfos.Select(x => x.Properties.Select(x => x.PropertyInfo.PropertyType.FullName).ToArray()).ToArray(),
                _6: typeInfos.Select(x => x.Properties.Select(x => x.Id).ToArray()).ToArray(),
                _7: typeInfos.Select(x => x.Properties.Select(x => x.Method.Symbol).ToArray()).ToArray(),
                _8: typeInfos.Select(x => x.Properties.Select(x => x.Method.Parameters.First().Symbol).ToArray()).ToArray(),
                _9: typeInfos.Select(x => x.Methods.Select(x => x.MethodInfo.Name).ToArray()).ToArray(),
                _10: typeInfos.Select(x => x.Methods.Select(x => x.Parameters.Select(x => x.ParameterInfo.Name).ToArray()).ToArray()).ToArray(),
                _11: typeInfos.Select(x => x.Methods.Select(x => x.Parameters.Select(x => x.ParameterInfo.ParameterType.FullName).ToArray()).ToArray()).ToArray(),
                _12: typeInfos.Select(x => x.Methods.Select(x => x.Parameters.Select(x => x.Id).ToArray()).ToArray()).ToArray(),
                _13: typeInfos.Select(x => x.Methods.Select(x => x.Symbol).ToArray()).ToArray(),
                _14: typeInfos.Select(x => x.Methods.Select(x => x.Parameters.Select(x => x.Symbol).ToArray()).ToArray()).ToArray()
            );
        }

        public object Resolve(Container container) {
            var gameObjectName = $"{Udon.Container.ImplementationType.Name} [{container.GetHashCode():x8}]";
            rootGo = new GameObject(gameObjectName);
            var transform = Destination.Transform?.Resolve<Transform>(container);
            if (transform != null) {
                rootGo.transform.SetParent(transform);
            }
            var ucontainer = (Udon.Container)rootGo.AddUdonSharpComponent(Udon.Container.ImplementationType, false);
            var containerData = BuildContainerData(container, ucontainer);
            var resolverData = BuildResolverData();
            var typeInfoData = BuildTypeInfoData();
            ucontainer.SetPublicVariable("_0", containerData._0);
            ucontainer.SetPublicVariable("_1", containerData._1);
            ucontainer.SetPublicVariable("_2", containerData._2);
            ucontainer.SetPublicVariable("_3", containerData._3);
            ucontainer.SetPublicVariable("_4", containerData._4);
            ucontainer.SetPublicVariable("_r0", resolverData._0);
            ucontainer.SetPublicVariable("_r1", resolverData._1);
            ucontainer.SetPublicVariable("_i0", typeInfoData._0);
            ucontainer.SetPublicVariable("_i1", typeInfoData._1);
            ucontainer.SetPublicVariable("_i2", typeInfoData._2);
            ucontainer.SetPublicVariable("_i3", typeInfoData._3);
            ucontainer.SetPublicVariable("_i4", typeInfoData._4);
            ucontainer.SetPublicVariable("_i5", typeInfoData._5);
            ucontainer.SetPublicVariable("_i6", typeInfoData._6);
            ucontainer.SetPublicVariable("_i7", typeInfoData._7);
            ucontainer.SetPublicVariable("_i8", typeInfoData._8);
            ucontainer.SetPublicVariable("_i9", typeInfoData._9);
            ucontainer.SetPublicVariable("_i10", typeInfoData._10);
            ucontainer.SetPublicVariable("_i11", typeInfoData._11);
            ucontainer.SetPublicVariable("_i12", typeInfoData._12);
            ucontainer.SetPublicVariable("_i13", typeInfoData._13);
            ucontainer.SetPublicVariable("_i14", typeInfoData._14);
            ucontainer.SetPublicVariable("_u0", transform?.gameObject.scene.GetRootGameObjects() ?? Array.Empty<GameObject>());
            ucontainer.ApplyProxyModifications();
            return ucontainer;
        }
    }
}
#endif
