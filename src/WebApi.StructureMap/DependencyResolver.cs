using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using StructureMap;

namespace WebApi.StructureMap
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public DependencyResolver(IContainer container)
        {
            _container = container;
            // Register the HttpControllerActivatorProxy so we can 
            // inject request scoped objects into the nested container
            // before the controller object graph is built.
            _container.Configure(x => x.For<IHttpControllerActivator>()
                .Use<HttpControllerActivatorProxy>());
        }

        public object GetService(Type serviceType)
        {
            // We only want resolve registered types from 
            // the container (So TryGetInstance, which returns 
            // null if not registered).
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

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
