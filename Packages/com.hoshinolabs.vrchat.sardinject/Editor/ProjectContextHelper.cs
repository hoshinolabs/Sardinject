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

namespace HoshinoLabs.VRC.Sardinject {
    internal static class ProjectContextHelper {
        static GameObject go;
        static Dictionary<Container, Udon.UdonContainer> cache;

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
            cache = new Dictionary<Container, Udon.UdonContainer>();

            context = new Context((Type type, Container scope) => {
                if (typeof(Udon.UdonContainer).IsAssignableFrom(type)) {
                    return GetOrBuildContainerUdon(scope);
                }
                return null;
            });
        }

        static GameObject GetOrBuildGO() {
            if (go == null) {
                go = new GameObject($"__{typeof(SardinjectBuilder).Namespace.Replace('.', '_')}__");
                go.hideFlags = HideFlags.HideAndDontSave;
            }

            return go;
        }

        static Udon.UdonContainer GetOrBuildContainerUdon(Container scope) {
            if (cache.TryGetValue(scope, out var udon)) {
                return udon;
            }

            var go = GetOrBuildGO();

            var containerGo = new GameObject($"__{typeof(Udon.UdonContainer).Name}_{scope.GetHashCode():x8}__");
            containerGo.transform.SetParent(go.transform);

            udon = containerGo.AddUdonSharpComponentEx<Udon.UdonContainer>(false);

            cache.Add(scope, udon);

            return udon;
        }

        public static void Build() {
            context.Build();

            foreach (var (container, udon) in cache) {
                var containerData = BuildContainerData(container, cache);
                udon.SetPublicVariable("_0", containerData._0);
                udon.SetPublicVariable("_1", containerData._1);
                udon.SetPublicVariable("_2", containerData._2);
                udon.SetPublicVariable("_3", containerData._3);
                udon.SetPublicVariable("cache", containerData.cache);
                UdonSharpEditorUtility.CopyProxyToUdon(udon, ProxySerializationPolicy.All);
            }
        }

        static (string[] _0, int[] _1, int[] _2, GameObject[] _3, DataDictionary cache) BuildContainerData(Container container, Dictionary<Container, Udon.UdonContainer> cache) {
            var containerCache = new DataDictionary();
            var containerData = container.Registry.Table
                .Select(x => {
                    var registration = x.Value;
                    var type = x.Key.ToString();
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
                    return (
                        type,
                        lifetime: (int)registration.Lifetime,
                        id,
                        obj
                    );
                })
                .ToList();
            return (
                _0: containerData.Select(x => x.type).ToArray(),
                _1: containerData.Select(x => x.lifetime).ToArray(),
                _2: containerData.Select(x => x.id).ToArray(),
                _3: containerData.Select(x => x.obj).ToArray(),
                containerCache
            );
        }
    }
}
