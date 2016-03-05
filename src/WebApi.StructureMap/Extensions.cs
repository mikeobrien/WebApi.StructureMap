using System;
using System.Web.Http;
using StructureMap;
using StructureMap.Graph;

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
    }
}
