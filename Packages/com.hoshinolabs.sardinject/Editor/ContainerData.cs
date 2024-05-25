using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.Sardinject {
    internal sealed class ContainerData {
        string type;
        Lifetime lifetime;
        int id;
        GameObject target;

        public string Type => type;
        public Lifetime Lifetime => lifetime;
        public int Id => id;
        public GameObject Target => target;

        internal ContainerData(string type, Lifetime lifetime, int id, GameObject target) {
            this.type = type;
            this.lifetime = lifetime;
            this.id = id;
            this.target = target;
        }
    }
}
