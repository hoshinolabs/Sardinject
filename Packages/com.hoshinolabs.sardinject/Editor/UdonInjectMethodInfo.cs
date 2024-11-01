using System.Reflection;

namespace HoshinoLabs.Sardinject {
    internal sealed class UdonInjectMethodInfo {
        public readonly MethodInfo MethodInfo;
        public readonly string Symbol;
        public readonly UdonInjectParameterInfo[] Parameters;

        public UdonInjectMethodInfo(MethodInfo methodInfo, string symbol, UdonInjectParameterInfo[] parameters) {
            MethodInfo = methodInfo;
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
