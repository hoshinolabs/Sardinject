using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public abstract class Scope : MonoBehaviour {
        List<Installer> installers = new();

        [SerializeField]
        GameObject[] autoInjectedGameObjects;

        public void Push(Action<ContainerBuilder> configuration) {
            installers.Add(new Installer(configuration));
        }

        public void InstallTo(ContainerBuilder builder) {
            foreach (var installer in GetComponents<IInstaller>().Concat(installers)) {
                installer.Install(builder);
            }
            var components = autoInjectedGameObjects
                .Where(x => x != null)
                .SelectMany(x => x.GetComponents<Component>());
            foreach (var component in components) {
                builder.RegisterComponentInstance(component);
            }
        }
    }
}
