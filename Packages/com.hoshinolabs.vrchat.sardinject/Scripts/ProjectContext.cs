using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public static class ProjectContext {
        static Context instance;

        public static Context New(Resolver resolver = null) {
            return instance.New(resolver);
        }

        public static void Build() {
            instance.Build();
        }

        public static void Enqueue(Action<IContainerBuilder> configuration) {
            instance.Enqueue(configuration);
        }
    }
}
