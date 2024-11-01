using System.Reflection;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectMethodInfo {
        public readonly MethodInfo MethodInfo;
        public readonly InjectParameterInfo[] Parameters;

        public InjectMethodInfo(MethodInfo methodInfo, InjectParameterInfo[] parameters) {
            MethodInfo = methodInfo;
            Parameters = parameters;
        }
    }
}
