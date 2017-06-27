using System;
using System.Collections.Generic;
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
            return context.Request.GetDependencyScope().GetService<T>(context,
                context.ActionContext, context.Response);
        }

        public static T GetService<T>(this HttpActionContext context)
        {
            return context.Request.GetDependencyScope().GetService<T>(context,
                context.ActionDescriptor, context.ControllerContext,
                context.ModelState);
        }

        private static T GetService<T>(this IDependencyScope scope,
            params object[] arguments)
        {
            var container = scope.GetService<IContainer>();
            var explicitArguments = new ExplicitArguments();
            arguments.ForEach(x => explicitArguments.Set(x.GetType(), x));
            return container.GetInstance<T>(explicitArguments);
        }

        private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
        }
    }
}
