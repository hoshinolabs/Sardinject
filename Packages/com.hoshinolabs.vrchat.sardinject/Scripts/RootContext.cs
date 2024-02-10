using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public sealed class RootContext : IContext {
        static Context instance;

        public static Context CreateChild() {
            return instance.CreateChild();
        }

        public static void Push(Action<ContainerBuilder> installing) {
            instance.Push(installing);
        }
    }
}
