using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace WebApi.StructureMap
{
    public class DependencyScope : IDependencyScope
    {
        private readonly IContainer _container;

        public DependencyScope(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            // We want resolve all types from the 
            // container (So GetInstance) when scoped to 
            // the request, as opposed to the dependency 
            // resolver where we only want to resolve 
            // registered types.
            return _container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }
    
        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
