using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ProjectContext {
        static Context instance;

        public static event Resolver Resolver {
            add => instance.Resolver += value;
            remove => instance.Resolver -= value;
        }

        public static Context New() {
            return instance.New();
        }

        public static void Build() {
            instance.Build();
        }

        public static void Enqueue(Action<IContainerBuilder> configuration) {
            instance.Enqueue(configuration);
        }
    }
}
