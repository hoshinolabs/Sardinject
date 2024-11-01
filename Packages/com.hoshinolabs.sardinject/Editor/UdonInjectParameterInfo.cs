using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonInjectParameterInfo {
        public readonly ParameterInfo ParameterInfo;
        public readonly string Symbol;
        public readonly object Id;

        public UdonInjectParameterInfo(ParameterInfo parameterInfo, string symbol, object id) {
            ParameterInfo = parameterInfo;
            Symbol = symbol;
            Id = id;
        }
    }
}
