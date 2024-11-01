#if UDONSHARP
using HoshinoLabs.Sardinject.Udon;
#endif
using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    internal sealed class SardinjectBuilder : IProcessSceneWithReport {
        public int callbackOrder => 1;

        GameObject rootGo;

        public void OnProcessScene(Scene scene, BuildReport report) {
            try {
                var sw = new Stopwatch();
                sw.Start();
                var projectContainer = BuildProjectContainer();
                var sceneContainer = BuildSceneContainer(scene, projectContainer);
                var containers = BuildHierarchyContainers(scene, sceneContainer);
                sw.Stop();
                Logger.Log($"Build of {containers.Length + 2} containers finished in {sw.Elapsed.ToString("hh\\:mm\\:ss")}");
            }
            catch (SardinjectException e) {
                Logger.LogError(e.Message);
                EditorApplication.ExitPlaymode();
                return;
            }
        }

#if UDONSHARP
        void OverrideUdonContainerInjection(ContainerBuilder containerBuilder) {
            var destination = new ComponentDestination();
            var resolverBuilder = new UdonContainerResolverBuilder(destination).OverrideScopeIfNeeded(containerBuilder, Lifetime.Cached);
            var builder = new ComponentBindingBuilder(typeof(IContainer), resolverBuilder, destination);
            builder.UnderTransform(() => {
                if (rootGo == null) {
                    rootGo = new GameObject($"__{GetType().Namespace.Replace('.', '_')}__");
                    //rootGo.hideFlags = HideFlags.HideInHierarchy;
                }
                return rootGo.transform;
            });
            containerBuilder.Register(builder);
        }
#endif

        Container BuildProjectContainer() {
            var builder = new ContainerBuilder();
            var scopes = Resources.LoadAll<ProjectScope>(string.Empty)
                .Where(x => x.gameObject.activeSelf);
            foreach (var scope in scopes) {
                scope.InstallTo(builder);
            }
#if UDONSHARP
            OverrideUdonContainerInjection(builder);
#endif
            return builder.Build();
        }

        Container BuildSceneContainer(Scene scene, Container projectContainer) {
            return projectContainer.Scope(builder => {
                var scopes = scene.GetRootGameObjects()
                    .SelectMany(x => x.GetComponentsInChildren<SceneScope>(true))
                    .Where(x => x.gameObject.activeSelf);
                foreach (var scope in scopes) {
                    scope.InstallTo(builder);
                    foreach (var installer in scope.GetComponents(typeof(IInstaller))) {
                        GameObject.DestroyImmediate(installer);
                    }
                    GameObject.DestroyImmediate(scope);
                }
#if UDONSHARP
                OverrideUdonContainerInjection(builder);
#endif
            });
        }

        Container[] BuildHierarchyContainers(Scene scene, Container sceneContainer) {
            return scene.GetRootGameObjects()
                .SelectMany(x => BuildHierarchyContainers(sceneContainer, x.transform))
                .ToArray();
        }

        Container[] BuildHierarchyContainers(Container rootContainer, Transform transform) {
            var hierarchy = transform.GetComponentInChildren<HierarchyScope>();
            if (hierarchy == null) {
                return Array.Empty<Container>();
            }
            var container = rootContainer.Scope(builder => {
                var scopes = hierarchy.GetComponents<HierarchyScope>();
                foreach (var scope in scopes) {
                    scope.InstallTo(builder);
                    foreach (var installer in scope.GetComponents(typeof(IInstaller))) {
                        GameObject.DestroyImmediate(installer);
                    }
                    GameObject.DestroyImmediate(scope);
                }
#if UDONSHARP
                OverrideUdonContainerInjection(builder);
#endif
            });
            var containers = Enumerable.Range(0, transform.childCount)
                .SelectMany(x => BuildHierarchyContainers(container, transform.GetChild(x)))
                .ToList();
            containers.Insert(0, container);
            return containers.ToArray();
        }
    }
}
