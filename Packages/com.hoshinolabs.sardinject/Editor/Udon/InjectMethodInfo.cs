using System.Reflection;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class InjectMethodInfo {
        public readonly MethodInfo MethodInfo;
        public readonly string Symbol;
        public readonly InjectParameterInfo[] Parameters;

        public InjectMethodInfo(MethodInfo methodInfo, string symbol, InjectParameterInfo[] parameters) {
            MethodInfo = methodInfo;
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
