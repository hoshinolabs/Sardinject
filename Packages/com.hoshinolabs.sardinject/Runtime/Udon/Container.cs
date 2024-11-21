using System;
using UdonSharp;
using UnityEngine;

namespace HoshinoLabs.Sardinject.Udon {
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [ImplementationType(typeof(ContainerShim))]
    public abstract class Container : UdonSharpBehaviour {
        public virtual object Resolve(Type type) => null;

        public virtual void Inject(object instance) { }
    }
}
