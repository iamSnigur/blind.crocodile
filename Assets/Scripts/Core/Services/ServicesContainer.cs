using System.Collections.Generic;
using System;

namespace BlindCrocodile.Core.Services
{
    public class ServicesContainer
    {
        public static ServicesContainer Instance => _instance ??= new ServicesContainer();

        private static ServicesContainer _instance;
        private static Dictionary<Type, IService> _services = new();

        private ServicesContainer() { }

        public void BindSingle<TImplementation>(IService service) =>
            _services.Add(typeof(TImplementation), service);

        public TImplementation Single<TImplementation>() where TImplementation : class =>
            _services[typeof(TImplementation)] as TImplementation;
    }
}
