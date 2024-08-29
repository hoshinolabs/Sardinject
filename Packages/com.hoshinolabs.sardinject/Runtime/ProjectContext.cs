using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ProjectContext {
        static Context context;

        public static event Resolver Resolver {
            add => context.Resolver += value;
            remove => context.Resolver -= value;
        }

        public static Container Container => context.Container;

        public static Context New() {
            return context.New();
        }

        public static Container Build() {
            return context.Build();
        }

        public static void Enqueue(Action<IContainerBuilder> configuration) {
            context.Enqueue(configuration);
        }
    }
}
