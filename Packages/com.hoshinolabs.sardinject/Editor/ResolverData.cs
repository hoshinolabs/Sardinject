namespace HoshinoLabs.Sardinject {
    internal sealed class ResolverData {
        public readonly string Signature;
        public readonly object[] Args;

        public ResolverData(string signature, object[] args) {
            Signature = signature;
            Args = args;
        }
    }
}
