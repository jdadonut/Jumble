

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Jumble.Util
{
    
    public class ServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _provider;
        private readonly object _service;

        public ServiceProvider(IServiceProvider provider, object service)
        {
            _provider = provider;
            _service = service;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(object))
            {
                return _service;
            }
            return _provider.GetService(serviceType);
        }
    }

    public static class ExtensionMethods
    {
        public static T GetFieldValue<T>(this object obj, string name) {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field?.GetValue(obj);
        }
    }
}

