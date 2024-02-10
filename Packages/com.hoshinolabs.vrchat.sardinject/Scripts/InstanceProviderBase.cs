using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public abstract class InstanceProviderBase {
        protected Hashtable parameters = new Hashtable();
        public Hashtable Parameters => parameters;
        protected Func<Transform> getTransform;
        public Func<Transform> GetTransform;

        protected Injector injector;
        protected Type implementationType;

        public void Build(Injector injector, Type implementationType) {
            this.injector = injector;
            this.implementationType = implementationType;
        }

        public abstract bool IsRaw { get; }
        public abstract bool IsPrefab { get; }

        public abstract object GetInstance(Container container);
    }
}
