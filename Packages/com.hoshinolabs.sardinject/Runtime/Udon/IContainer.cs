using System;
using UdonSharp;
using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class IContainer : UdonSharpBehaviour {
        public static Type ImplementationType => typeof(Container);

        public virtual object Resolve(Type type) => null;

        public virtual void Inject(object instance) { }
    }
}
