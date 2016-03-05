using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using StructureMap;

namespace WebApi.StructureMap
{
    // This allows us to inject request scoped objects into the 
    // nested container before the controller object graph is built.
    public class HttpControllerActivatorProxy : IHttpControllerActivator
    {
        public IHttpController Create(
            HttpRequestMessage request, 
            HttpControllerDescriptor controllerDescriptor, 
            Type controllerType)
        {
            request.GetDependencyScope().GetService<IContainer>().Configure(x =>
            {
                x.For<HttpRequestMessage>().Use(request);
                x.For<HttpControllerDescriptor>().Use(controllerDescriptor);
                x.For<HttpRequestContext>().Use(request.GetRequestContext());
                x.For<IHttpRouteData>().Use(request.GetRouteData());
            });

            return new DefaultHttpControllerActivator()
                .Create(request, controllerDescriptor, controllerType);
        }
    }
}
