using System.Reflection;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class InjectParameterInfo {
        public readonly ParameterInfo ParameterInfo;
        public readonly string Symbol;
        public readonly object Id;

        public InjectParameterInfo(ParameterInfo parameterInfo, string symbol, object id) {
            ParameterInfo = parameterInfo;
            Symbol = symbol;
            Id = id;
        }
    }
}
