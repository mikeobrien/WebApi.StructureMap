using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace WebApi.StructureMap
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public DependencyResolver(IContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public object GetService(Type serviceType)
        {
            // We only want resolve registered types from 
            // the container (So TryGetInstance, which returns 
            // null if not registered) when not scoped to the 
            // request, as opposed to the dependency scope 
            // where we want to resolve all types.
            return _container.TryGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        public IDependencyScope BeginScope()
        {
            return new DependencyScope(_container.GetNestedContainer());
        }
    }
}
