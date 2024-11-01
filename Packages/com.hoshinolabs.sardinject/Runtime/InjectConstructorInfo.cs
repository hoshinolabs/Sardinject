using System.Reflection;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectConstructorInfo {
        public readonly ConstructorInfo ConstructorInfo;
        public readonly InjectParameterInfo[] Parameters;

        public InjectConstructorInfo(ConstructorInfo constructorInfo, InjectParameterInfo[] parameters) {
            ConstructorInfo = constructorInfo;
            Parameters = parameters;
        }
    }
}
