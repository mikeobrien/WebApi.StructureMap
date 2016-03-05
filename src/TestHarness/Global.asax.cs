using System;
using System.Web.Http;
using WebApi.StructureMap;

namespace TestHarness
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.Filters.Add(new TestFilter());
            GlobalConfiguration.Configuration.UseStructureMap<Registry>();
            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}