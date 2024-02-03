using System.Collections.Generic;
using System;

namespace BlindCrocodile.Core.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;
        private static readonly Dictionary<Type, IService> _services = new();

        private ServiceLocator() { }

        public void BindSingle<TImplementation>(IService service) =>
            _services.Add(typeof(TImplementation), service);

        public TImplementation Single<TImplementation>() where TImplementation : class =>
            _services[typeof(TImplementation)] as TImplementation;
    }
}
