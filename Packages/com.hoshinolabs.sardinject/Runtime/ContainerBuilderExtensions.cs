using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ContainerBuilderExtensions {
        /// <summary>
        /// Add entry point by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        //public static RegistrationBuilder AddEntryPoint<T>(this IContainerBuilder self) where T : Component {
        //    return self.AddInHierarchy<T>();
        //}
        public static void AddEntryPoint<T>(this IContainerBuilder self) where T : Component {
            self.OnBuild += container => {
                container.Resolve<T>();
            };
        }

        /// <summary>
        /// Add by instance and type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddInstance<T>(this IContainerBuilder self, T instance) where T : Component {
            var provider = new InstanceProvider(instance);
            var builder = new RegistrationBuilder(typeof(T), Lifetime.Cached, provider).As<T>();
            self.OnBuild += container => {
                container.Resolve(typeof(T));
            };
            return self.Register(builder);
        }

        /// <summary>
        /// Find and add by type from the hierarchy
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddInHierarchy(this IContainerBuilder self, Type type) {
            var provider = new FindInstanceProvider();
            var builder = new RegistrationBuilder(type, Lifetime.Cached, provider);
            self.OnBuild += container => {
                container.Resolve(builder.InterfaceTypes.FirstOrDefault() ?? builder.ImplementationType);
            };
            return self.Register(builder);
        }

        /// <summary>
        /// Find and add by type from the hierarchy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddInHierarchy<T>(this IContainerBuilder self) where T : Component {
            return self.AddInHierarchy(typeof(T));
        }

        /// <summary>
        /// Create and add from prefab
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <param name="lifetime"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddInNewPrefab(this IContainerBuilder self, Type type, Lifetime lifetime, GameObject prefab) {
            var provider = new PrefabInstanceProvider(prefab);
            var builder = new RegistrationBuilder(type, lifetime, provider);
            return self.Register(builder);
        }

        /// <summary>
        /// Create and add from prefab
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="lifetime"></param>
        /// <param name="gameObjectName"></param>
        public static RegistrationBuilder AddInNewPrefab<T>(this IContainerBuilder self, Lifetime lifetime, GameObject prefab) where T : Component {
            return self.AddInNewPrefab(typeof(T), lifetime, prefab);
        }

        /// <summary>
        /// Create objects and add them with components
        /// </summary>
        /// <param name="self"></param>
        /// <param name="type"></param>
        /// <param name="lifetime"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddOnNewGameObject(this IContainerBuilder self, Type type, Lifetime lifetime, string gameObjectName = null) {
            var provider = new NewGameObjectInstanceProvider(gameObjectName);
            var builder = new RegistrationBuilder(type, lifetime, provider);
            return self.Register(builder);
        }

        /// <summary>
        /// Create objects and add them with components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="lifetime"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static RegistrationBuilder AddOnNewGameObject<T>(this IContainerBuilder self, Lifetime lifetime, string gameObjectName = null) where T : Component {
            return self.AddOnNewGameObject(typeof(T), lifetime, gameObjectName);
        }
    }
}
