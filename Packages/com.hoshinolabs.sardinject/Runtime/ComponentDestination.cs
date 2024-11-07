using UnityEngine;

namespace HoshinoLabs.Sardinject {
    public sealed class ComponentDestination {
        public IResolver Transform = new ParameterResolver(null);
        public bool DontDestroyOnLoad;

        public void ApplyDontDestroyOnLoadIfNeeded(Component component) {
            if (DontDestroyOnLoad) {
                GameObject.DontDestroyOnLoad(component);
            }
        }
    }
}
