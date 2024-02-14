using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Data;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class SardinjectBuilder : IProcessSceneWithReport {
        public int callbackOrder => 1;

        static GameObject go;
        static Dictionary<Container, Udon.UdonContainer> cache;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            go = null;
            cache = new Dictionary<Container, Udon.UdonContainer>();
        }

        static GameObject GetOrBuildGO() {
            if (go == null) {
                go = new GameObject($"__{typeof(SardinjectBuilder).Namespace.Replace('.', '_')}__");
                //go.hideFlags = HideFlags.HideAndDontSave;
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

        internal static object FallbackResolver(Type type, Container scope) {
            if (typeof(Udon.UdonContainer).IsAssignableFrom(type)) {
                return GetOrBuildContainerUdon(scope);
            }
            return null;
        }

        public void OnProcessScene(Scene scene, BuildReport report) {
            try {
                ProjectContextHelper.Instance.Build();
            }
            catch (SardinjectException e) {
                EditorApplication.isPlaying = false;
                Debug.LogError($"[<color=#47F1FF>Sardinject</color>] {e.Message}");

                throw e;
            }

            foreach(var (container, udon) in cache) {
                var containerData = BuildContainerData(container, cache);
                udon.SetPublicVariable("_0", containerData._0);
                udon.SetPublicVariable("_1", containerData._1);
                udon.SetPublicVariable("_2", containerData._2);
                udon.SetPublicVariable("_3", containerData._3);
                udon.SetPublicVariable("cache", containerData.cache);
                UdonSharpEditorUtility.CopyProxyToUdon(udon, ProxySerializationPolicy.All);
            }
        }

        (string[] _0, int[] _1, int[] _2, GameObject[] _3, DataDictionary cache) BuildContainerData(Container container, Dictionary<Container, Udon.UdonContainer> cache) {
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
