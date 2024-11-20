using System;
using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    internal sealed class OpenGenericInstanceResolverBuilder : IGenericResolverBuilder {
        public Dictionary<object, IResolver> Parameters { get; } = new();

        public OpenGenericInstanceResolverBuilder() {

        }

        public IBindingResolver Build() {
            return new OpenGenericInstanceResolver(Parameters);
        }
    }
}
