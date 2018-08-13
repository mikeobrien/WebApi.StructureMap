using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace WebApi.StructureMap
{
    public static class Extensions
    {
        public static void UseStructureMap(
            this HttpConfiguration config,
            Action<ConfigurationExpression> configuration)
        {
            config.DependencyResolver = new DependencyResolver(
                new Container(configuration));
        }

        public static void UseStructureMap(
            this HttpConfiguration config,
            PluginGraph graph)
        {
            config.DependencyResolver = new DependencyResolver(
                new Container(graph));
        }

        public static void UseStructureMap(
            this HttpConfiguration config,
            Registry registry)
        {
            config.DependencyResolver = new DependencyResolver(
                new Container(registry));
        }

        public static void UseStructureMap<T>(
            this HttpConfiguration config)
            where T : Registry, new()
        {
            config.UseStructureMap(new T());
        }

        public static void UseStructureMap(
                this HttpConfiguration config,
                IContainer container)
        {
            config.DependencyResolver = new DependencyResolver(container);
        }

        public static T GetService<T>(this IDependencyScope scope)
        {
            return (T)scope.GetService(typeof(T));
        }

        public static T GetService<T>(this HttpRequestMessage message)
        {
            return message.GetDependencyScope().GetService<T>();
        }

        public static T GetService<T>(this HttpActionExecutedContext context)
        {
            var scope = context.Request.GetDependencyScope();
            var container = scope.GetService<IContainer>();

            var explicitArguments = new ExplicitArguments();
            explicitArguments.SetWithActualType(context);
            explicitArguments.SetWithActualType(context.ActionContext);
            explicitArguments.SetWithActualType(context.Response);

            return container.GetInstance<T>(explicitArguments);
        }

        public static T GetService<T>(this HttpActionContext context)
        {
            var scope = context.Request.GetDependencyScope();
            var container = scope.GetService<IContainer>();

            var explicitArguments = new ExplicitArguments();
            explicitArguments.SetWithActualType(context);
            explicitArguments.SetWithActualType(context.ActionDescriptor);
            explicitArguments.SetWithActualType(context.ControllerContext);
            explicitArguments.SetWithActualType(context.ModelState);

            return container.GetInstance<T>(explicitArguments);
        }

        private static void SetWithActualType<T>(this ExplicitArguments explicitArguments, T instance)
        {
            explicitArguments.Set(instance == null ? typeof(T) : instance.GetType(), instance);
        }
    }
}
