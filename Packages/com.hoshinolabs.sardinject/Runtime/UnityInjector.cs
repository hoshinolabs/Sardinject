using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public static class UnityInjector {
        public static event Action<ContainerBuilder> Installers;
        public static event Action<Container> OnProjectContainerBuilt;
        public static event Action<Scene, Container> OnSceneContainerBuilt;
        public static event Action<Transform, Container> OnHierarchyContainerBuilt;

        static Container projectContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnSubsystemRegistration() {
            projectContainer = BuildProjectContainer();

            SceneInjector.OnSceneLoaded -= SceneLoaded;
            SceneInjector.OnSceneLoaded += SceneLoaded;
        }

        static void SceneLoaded(Scene scene) {
            var sceneContainer = BuildSceneContainer(projectContainer, scene);
            BuildHierarchyContainers(sceneContainer, scene);
        }

        static Container BuildProjectContainer() {
            var sw = Stopwatch.StartNew();
            var builder = new ContainerBuilder();
            var scopes = Resources.LoadAll<ProjectScope>(string.Empty)
                .Where(x => x.gameObject.activeSelf);
            foreach (var scope in scopes) {
                scope.InstallTo(builder);
            }
            Installers?.Invoke(builder);
            var container = builder.Build();
            OnProjectContainerBuilt?.Invoke(container);
            Logger.Log($"Build of project container finished in {sw.Elapsed.TotalMilliseconds}ms");
            return container;
        }

        static Container BuildSceneContainer(Container projectContainer, Scene scene) {
            var sw = Stopwatch.StartNew();
            var container = projectContainer.Scope(builder => {
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
                Installers?.Invoke(builder);
            });
            OnSceneContainerBuilt?.Invoke(scene, container);
            Logger.Log($"Build of scene container finished in {sw.Elapsed.TotalMilliseconds}ms");
            return container;
        }

        static Container[] BuildHierarchyContainers(Container sceneContainer, Scene scene) {
            var sw = Stopwatch.StartNew();
            var containers = scene.GetRootGameObjects()
                .SelectMany(x => BuildHierarchyContainers(sceneContainer, x.transform))
                .ToArray();
            if (0 < containers.Length) {
                Logger.Log($"Builds of {containers.Length} hierarchy containers finished in {sw.Elapsed.TotalMilliseconds}ms");
            }
            return containers;
        }

        static Container[] BuildHierarchyContainers(Container rootContainer, Transform transform) {
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
                Installers?.Invoke(builder);
            });
            OnHierarchyContainerBuilt?.Invoke(transform, container);
            var containers = Enumerable.Range(0, transform.childCount)
                .SelectMany(x => BuildHierarchyContainers(container, transform.GetChild(x)))
                .ToList();
            containers.Insert(0, container);
            return containers.ToArray();
        }
    }
}
