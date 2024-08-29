using HoshinoLabs.Sardinject.Udon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoshinoLabs.Sardinject {
    internal sealed class SardinjectPreBuilder : IProcessSceneWithReport {
        public int callbackOrder => -5001;

        public void OnProcessScene(Scene scene, BuildReport report) {
            SardinjectHelper.Go = null;
            SardinjectHelper.ContainerCache = new Dictionary<Container, IContainer>();

            SardinjectHelper.Context = new Context();
            SardinjectHelper.Context.Resolver += (Container container, Type type) => {
                if (typeof(IContainer).IsAssignableFrom(type)) {
                    return GetOrContainer(container);
                }
                return null;
            };
        }

        Transform GetOrUnderTransform() {
            if(SardinjectHelper.Go != null) {
                return SardinjectHelper.Go.transform;
            }
            SardinjectHelper.Go = new GameObject($"__{GetType().Namespace.Replace('.', '_')}__");
            SardinjectHelper.Go.hideFlags = HideFlags.HideInHierarchy;
            return SardinjectHelper.Go.transform;
        }

        IContainer GetOrContainer(Container container) {
            if (SardinjectHelper.ContainerCache.TryGetValue(container, out var udon)) {
                return udon;
            }
            var go = new GameObject($"{IContainer.ImplementationType.Name} [{container.GetHashCode():x8}]");
            go.transform.SetParent(GetOrUnderTransform());
            udon = (IContainer)go.AddUdonSharpComponentEx(IContainer.ImplementationType, false);
            SardinjectHelper.ContainerCache.Add(container, udon);
            return udon;
        }
    }
}
