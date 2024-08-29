using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HoshinoLabs.Sardinject.Udon {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("")]
    public class IContainer : UdonSharpBehaviour {
        public static Type ImplementationType => typeof(Container);

        public virtual object Resolve(string type) => null;
        public virtual object Resolve(Type type) => null;

        public virtual void Inject(object instance) { }
    }
}
