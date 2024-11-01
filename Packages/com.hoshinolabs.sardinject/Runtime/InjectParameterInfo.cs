using System.Reflection;

namespace HoshinoLabs.Sardinject {
    public sealed class InjectParameterInfo {
        public readonly ParameterInfo ParameterInfo;
        public readonly object Id;

        public InjectParameterInfo(ParameterInfo parameterInfo, object id) {
            ParameterInfo = parameterInfo;
            Id = id;
        }
    }
}
