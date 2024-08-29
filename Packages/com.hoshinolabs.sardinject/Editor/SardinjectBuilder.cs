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

namespace HoshinoLabs.Sardinject {
    internal sealed class SardinjectBuilder : IProcessSceneWithReport {
        public int callbackOrder => 1;

        public void OnProcessScene(Scene scene, BuildReport report) {
            try {
                SardinjectHelper.Context.Build();
            }
            catch (SardinjectException e) {
                //EditorApplication.isPlaying = false;
                //Debug.LogError($"[<color=#47F1FF>Sardinject</color>] {e.Message}");

                //throw e;
                throw new BuildFailedException(e);
            }

            foreach (var (container, udon) in SardinjectHelper.ContainerCache) {
                var containerData = BuildContainerData(container);
                udon.SetPublicVariable("_0", containerData._0);
                udon.SetPublicVariable("_1", containerData._1);
                udon.SetPublicVariable("_2", containerData._2);
                udon.SetPublicVariable("_3", containerData._3);
                udon.SetPublicVariable("cache", containerData.cache);
                UdonSharpEditorUtility.CopyProxyToUdon(udon, ProxySerializationPolicy.All);
            }
        }

        ContainerData BuildContainerData(DataDictionary containerCache, Type registrationType, Registration registration, Container container) {
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
                        if (SardinjectHelper.ContainerCache.TryGetValue((Container)instance, out var udon)) {
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

        (string[] _0, int[] _1, int[] _2, GameObject[] _3, DataDictionary cache) BuildContainerData(Container container) {
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
