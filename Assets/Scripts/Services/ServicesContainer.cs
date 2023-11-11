using System;
using System.Collections.Generic;

namespace Scripts.Services
{
    public class ServicesContainer
    {
        public static ServicesContainer Instance
        {
            get
            {
                _instance ??= new ServicesContainer();

                return _instance;
            }
        }

        private static ServicesContainer _instance;

        private static Dictionary<Type, IService> _services = new();

        private ServicesContainer() { }

        public static void AsSingle<TImplementation>(IService service)
        {
            _services.Add(typeof(TImplementation), service);
        }

        public static IService Single<TImplementation>() =>
            _services[typeof(TImplementation)];
    }
}
