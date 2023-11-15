using System.Collections.Generic;
using System;

namespace BlindCrocodile.Services
{
    public class ServicesContainer
    {
        public static ServicesContainer Instance => _instance ??= new ServicesContainer();

        private static ServicesContainer _instance;

        private static Dictionary<Type, IService> _services = new();

        private ServicesContainer() { }

        public static void SingleAs<TImplementation>(IService service) =>
            _services.Add(typeof(TImplementation), service);

        public static IService Single<TImplementation>() =>
            _services[typeof(TImplementation)];
    }
}
