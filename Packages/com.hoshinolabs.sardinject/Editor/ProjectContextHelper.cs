using HoshinoLabs.Sardinject.Udon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase.Editor.BuildPipeline;

namespace HoshinoLabs.Sardinject {
    internal static class ProjectContextHelper {
        static GameObject go;
        static Dictionary<Container, IContainer> cache;

        static Context context {
            get {
                var instance = typeof(ProjectContext).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
                return (Context)instance.GetValue(null);
            }
            set {
                var instance = typeof(ProjectContext).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
                instance.SetValue(null, value);
            }
        }

        internal class BuildInitializer : IVRCSDKBuildRequestedCallback {
            public int callbackOrder => 0;

            public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType) {
                Init();
                return true;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            go = null;
            cache = new Dictionary<Container, IContainer>();

            context = new Context();
            context.Resolver += Resolver;
        }

        static object Resolver(Container container, Type type, IEnumerable<Attribute> attributes) {
            if (typeof(IContainer).IsAssignableFrom(type)) {
                return GetOrBuildContainerUdon(container);
            }
            return null;
        }

        static GameObject GetOrBuildGO() {
            if (go == null) {
                go = new GameObject($"__{typeof(SardinjectBuilder).Namespace.Replace('.', '_')}__");
                go.hideFlags = HideFlags.HideInHierarchy;
            }

            return go;
        }

        static IContainer GetOrBuildContainerUdon(Container scope) {
            if (cache.TryGetValue(scope, out var udon)) {
                return udon;
            }

            var go = GetOrBuildGO();

            var containerGo = new GameObject($"{ContainerTypeResolver.ImplementationType.Name} [{scope.GetHashCode():x8}]");
            containerGo.transform.SetParent(go.transform);

            udon = (IContainer)containerGo.AddUdonSharpComponentEx(ContainerTypeResolver.ImplementationType, false);

            cache.Add(scope, udon);

            return udon;
        }

        public static void Build() {
            context.Build();

            foreach (var (container, udon) in cache) {
                var containerData = BuildContainerData(container);
                udon.SetPublicVariable("_0", containerData._0);
                udon.SetPublicVariable("_1", containerData._1);
                udon.SetPublicVariable("_2", containerData._2);
                udon.SetPublicVariable("_3", containerData._3);
                udon.SetPublicVariable("cache", containerData.cache);
                UdonSharpEditorUtility.CopyProxyToUdon(udon, ProxySerializationPolicy.All);
            }
        }

        static ContainerData BuildContainerData(DataDictionary containerCache, Type registrationType, Registration registration, Container container) {
            var type = registrationType.ToString();
            var id = registration.GetHashCode();
            var obj = default(GameObject);
            if (registration.IsPrefab) {
                obj = (GameObject)registration.GetInstance(container);
            }
            if (registration.IsRaw) {
                var instance = registration.GetInstance(container);
                if (instance != null) {
                    if (typeof(Container).IsAssignableFrom(instance.GetType())) {
                        if (cache.TryGetValue((Container)instance, out var udon)) {
                            type = udon.GetType().ToString();
                            instance = udon;
                        }
                    }
                    if (typeof(UdonSharpBehaviour).IsAssignableFrom(instance.GetType())) {
                        instance = UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)instance);
                    }
                    if (typeof(Component).IsAssignableFrom(instance.GetType())) {
                        containerCache.Add(id, (Component)instance);
                    }
                }
            }
            return new ContainerData(type, registration.Lifetime, id, obj);
        }

        static (string[] _0, int[] _1, int[] _2, GameObject[] _3, DataDictionary cache) BuildContainerData(Container container) {
            var containerCache = new DataDictionary();
            var containerData = container.Registry.Table
                .SelectMany(x => x.Value.Select(Value => (x.Key, Value)))
                .Select(x => BuildContainerData(containerCache, x.Key, x.Value, container))
                .ToList();
            return (
                _0: containerData.Select(x => x.Type).ToArray(),
                _1: containerData.Select(x => (int)x.Lifetime).ToArray(),
                _2: containerData.Select(x => x.Id).ToArray(),
                _3: containerData.Select(x => x.Target).ToArray(),
                containerCache
            );
        }
    }
}
