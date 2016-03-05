using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi.StructureMap;

namespace TestHarness
{
    public class TestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.GetService<DummyService>().DoSomething();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.GetService<HeaderService>().SetHeaders();
        }
    }

    public class DummyService
    {
        public DummyService(
            HttpActionExecutedContext context,
            ISingletonDependency singletonDependency,
            ITransientDependency transientDependency) { }

        public void DoSomething() { }
    }

    public class HeaderService
    {
        private readonly HttpResponseMessage _response;
        private readonly ISingletonDependency _singletonDependency;
        private readonly ITransientDependency _transientDependency;

        public HeaderService(
            HttpResponseMessage response,
            ISingletonDependency singletonDependency,
            ITransientDependency transientDependency)
        {
            _response = response;
            _singletonDependency = singletonDependency;
            _transientDependency = transientDependency;
        }

        public void SetHeaders()
        {
            _response.Headers.Add("SingletonInstance",
                _singletonDependency.GetHashCode().ToString());
            _response.Headers.Add("TransientInstance",
                _transientDependency.GetHashCode().ToString());
        }
    }
}