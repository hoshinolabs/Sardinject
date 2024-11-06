using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    public static class UnityInjector {
        public static event Action<ContainerBuilder> Installers;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnSubsystemRegistration() {
            var sw = new Stopwatch();
            sw.Start();
            var projectContainer = BuildProjectContainer();
            sw.Stop();
            Logger.Log($"Build of project container finished in {sw.Elapsed.TotalMilliseconds}ms");

            void _SceneLoaded(Scene scene) {
                sw.Start();
                var sceneContainer = BuildSceneContainer(projectContainer, scene);
                sw.Stop();
                Logger.Log($"Build of scene container finished in {sw.Elapsed.TotalMilliseconds}ms");

                sw.Start();
                var containers = BuildHierarchyContainers(sceneContainer, scene);
                sw.Stop();
                if (0 < containers.Length) {
                    Logger.Log($"Builds of {containers.Length} hierarchy containers finished in {sw.Elapsed.TotalMilliseconds}ms");
                }
            };

            SceneInjector.OnSceneLoaded -= _SceneLoaded;
            SceneInjector.OnSceneLoaded += _SceneLoaded;
        }

        static Container BuildProjectContainer() {
            var builder = new ContainerBuilder();
            var scopes = Resources.LoadAll<ProjectScope>(string.Empty)
                .Where(x => x.gameObject.activeSelf);
            foreach (var scope in scopes) {
                scope.InstallTo(builder);
            }
            Installers.Invoke(builder);
            return builder.Build();
        }

        static Container BuildSceneContainer(Container projectContainer, Scene scene) {
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
                Installers.Invoke(builder);
            });
        }

        static Container[] BuildHierarchyContainers(Container sceneContainer, Scene scene) {
            return scene.GetRootGameObjects()
                .SelectMany(x => BuildHierarchyContainers(sceneContainer, x.transform))
                .ToArray();
        }

        static Container[] BuildHierarchyContainers(Container rootContainer, Transform transform) {
            var hierarchy = transform.GetComponentInChildren<HierarchyScope>();
            if (hierarchy == null) {
                return Array.Empty<Container>();
            }
            var container = rootContainer.Scope(builder => {
                // TODO: Ç±ÇÍÇæÇ∆äKëwÇà€éùÇ≈Ç´Ç»Ç¢ÇÊÇ§Ç»ÅcÅH
                var scopes = hierarchy.GetComponents<HierarchyScope>();
                foreach (var scope in scopes) {
                    scope.InstallTo(builder);
                    foreach (var installer in scope.GetComponents(typeof(IInstaller))) {
                        GameObject.DestroyImmediate(installer);
                    }
                    GameObject.DestroyImmediate(scope);
                }
                Installers.Invoke(builder);
            });
            var containers = Enumerable.Range(0, transform.childCount)
                .SelectMany(x => BuildHierarchyContainers(container, transform.GetChild(x)))
                .ToList();
            containers.Insert(0, container);
            return containers.ToArray();
        }
    }
}
