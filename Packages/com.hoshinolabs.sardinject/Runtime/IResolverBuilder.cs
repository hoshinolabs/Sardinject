using System.Collections.Generic;

namespace HoshinoLabs.Sardinject {
    public interface IResolverBuilder {
        Dictionary<object, IResolver> Parameters { get; }

        IBindingResolver Build();
    }

    public interface IGenericResolverBuilder : IResolverBuilder {

    }
}
